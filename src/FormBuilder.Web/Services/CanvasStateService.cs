using FormBuilder.Web.Models.Designer;

namespace FormBuilder.Web.Services;

/// <summary>
/// Scoped (per-circuit) state for the Form Designer: the editing model, the active
/// page, the selected field, layout-mode flag, and all canvas mutation operations.
/// Drop rules mirror the HTML prototype (layout-mode nesting, field placement).
/// </summary>
public class CanvasStateService
{
    private readonly DragDropStateService _drag;
    private readonly LayoutDesignerService _layout;

    public CanvasStateService(DragDropStateService drag, LayoutDesignerService layout)
    {
        _drag = drag;
        _layout = layout;
    }

    public DesignerForm? Form { get; private set; }
    public int ActivePage { get; private set; }
    public DesignerField? Selected { get; private set; }

    /// <summary>True when the Layout toolbox tab is active. Fields are non-interactive; layout drops nest.</summary>
    public bool LayoutMode { get; private set; }
    public bool Dirty { get; private set; }

    public event Action? Changed;

    public DesignerPage? Page => Form is null ? null : Form.Pages[ActivePage];

    public void Load(DesignerForm form)
    {
        Form = form;
        ActivePage = 0;
        Selected = null;
        Dirty = false;
        Notify();
    }

    public void SetActivePage(int index)
    {
        ActivePage = index;
        Selected = null;
        Notify();
    }

    public void SetLayoutMode(bool on)
    {
        LayoutMode = on;
        if (on) Selected = null; // can't edit fields in layout mode
        Notify();
    }

    public void Select(DesignerField field)
    {
        if (LayoutMode) return;          // selection/editing disabled in layout mode
        Selected = field;
        Notify();
    }

    public void ClearSelection()
    {
        Selected = null;
        Notify();
    }

    public void MarkDirty() { Dirty = true; Notify(); }
    public void ClearDirty() { Dirty = false; Notify(); }

    // ---- Drop handling (uses the current drag operation) ----

    /// <summary>Handle a drop onto a column placeholder (the only valid target for fields).</summary>
    public void DropOnColumn(DesignerColumn column, List<DesignerRow> parentRows, int rowIndex)
    {
        switch (_drag.Current)
        {
            case NewFieldDrag nf:
                var field = _layout.CreateField(nf.Type, TotalFieldCount());
                column.Fields.Add(field);
                Selected = LayoutMode ? null : field;
                Dirty = true;
                break;

            case MoveFieldDrag mf when mf.Source != column:
                mf.Source.Fields.Remove(mf.Field);
                column.Fields.Add(mf.Field);
                Dirty = true;
                break;

            case LayoutDrag ld when LayoutMode:
                // Layout mode: nest a sub-row inside this column.
                column.SubRows.Add(_layout.BuildRow(ld.Template));
                Dirty = true;
                break;

            case LayoutDrag ld:
                // Non-layout mode: insert a new row after this row (prototype behaviour).
                parentRows.Insert(Math.Clamp(rowIndex + 1, 0, parentRows.Count), _layout.BuildRow(ld.Template));
                Dirty = true;
                break;
        }
        _drag.End();
        Notify();
    }

    /// <summary>Handle a drop onto a row drop-zone (layout insert or row reorder).</summary>
    public void DropOnZone(List<DesignerRow> parentRows, int insertIndex)
    {
        switch (_drag.Current)
        {
            case LayoutDrag ld:
                parentRows.Insert(Math.Clamp(insertIndex, 0, parentRows.Count), _layout.BuildRow(ld.Template));
                Dirty = true;
                break;

            case RowMoveDrag rm when ReferenceEquals(rm.Parent, parentRows):
                var from = parentRows.IndexOf(rm.Row);
                if (from >= 0)
                {
                    parentRows.RemoveAt(from);
                    var adj = from < insertIndex ? insertIndex - 1 : insertIndex;
                    parentRows.Insert(Math.Clamp(adj, 0, parentRows.Count), rm.Row);
                    Dirty = true;
                }
                break;
        }
        _drag.End();
        Notify();
    }

    public void DeleteRow(List<DesignerRow> parentRows, DesignerRow row)
    {
        parentRows.Remove(row);
        Dirty = true;
        Notify();
    }

    public void DeleteField(DesignerColumn column, DesignerField field)
    {
        column.Fields.Remove(field);
        if (Selected == field) Selected = null;
        Dirty = true;
        Notify();
    }

    // ---- Field enumeration (recursive, includes nested sub-rows) ----

    public int TotalFieldCount() => Form is null ? 0 : AllFields().Count();

    public IEnumerable<DesignerField> AllFields()
        => Page is null ? Enumerable.Empty<DesignerField>() : Page.Rows.SelectMany(EnumerateRow);

    public int PageFieldCount(DesignerPage page) => page.Rows.SelectMany(EnumerateRow).Count();

    private static IEnumerable<DesignerField> EnumerateRow(DesignerRow row)
        => row.Columns.SelectMany(c => c.Fields.Concat(c.SubRows.SelectMany(EnumerateRow)));

    private void Notify() => Changed?.Invoke();
}
