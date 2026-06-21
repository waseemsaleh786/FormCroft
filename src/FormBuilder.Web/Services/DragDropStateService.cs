using FormBuilder.Domain.Enums;
using FormBuilder.Web.Models.Designer;

namespace FormBuilder.Web.Services;

// ---- Strongly-typed drag operations (what is currently being dragged) ----
public abstract record DragOperation;

/// <summary>Dragging a new field type from the Fields toolbox.</summary>
public sealed record NewFieldDrag(FieldType Type) : DragOperation;

/// <summary>Dragging a layout template from the Layout toolbox.</summary>
public sealed record LayoutDrag(LayoutTemplate Template) : DragOperation;

/// <summary>Dragging an existing field on the canvas to move it.</summary>
public sealed record MoveFieldDrag(DesignerField Field, DesignerColumn Source) : DragOperation;

/// <summary>Dragging a row by its handle to reorder it within its parent list.</summary>
public sealed record RowMoveDrag(DesignerRow Row, List<DesignerRow> Parent) : DragOperation;

/// <summary>
/// Scoped (per-circuit) holder for the active drag operation and the current
/// drag-over target id. Components subscribe to <see cref="Changed"/> to re-render
/// highlight state (native HTML5 ':hover' does not fire mid-drag).
/// </summary>
public class DragDropStateService
{
    public DragOperation? Current { get; private set; }

    /// <summary>Identifier of the drop target currently under the cursor.</summary>
    public string? OverTargetId { get; private set; }

    public event Action? Changed;

    public bool IsDragging => Current is not null;
    public bool IsLayoutDrag => Current is LayoutDrag;
    public bool IsFieldDrag => Current is NewFieldDrag or MoveFieldDrag;

    public void Start(DragOperation operation)
    {
        Current = operation;
        Notify();
    }

    public void SetOver(string? targetId)
    {
        if (OverTargetId == targetId) return;
        OverTargetId = targetId;
        Notify();
    }

    /// <summary>Clears the over-target only if it still matches (avoids dragleave flicker).</summary>
    public void ClearOver(string targetId)
    {
        if (OverTargetId != targetId) return;
        OverTargetId = null;
        Notify();
    }

    public void End()
    {
        Current = null;
        OverTargetId = null;
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
