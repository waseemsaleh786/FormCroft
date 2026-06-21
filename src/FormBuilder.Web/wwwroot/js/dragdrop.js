// Enables HTML5 drop targets for the Form Designer.
// Blazor Server's `@ondragover:preventDefault` (without a handler delegate) does not
// reliably call preventDefault, so the browser rejects drops. This global delegated
// listener calls preventDefault natively on dragover/dragenter over any designer drop
// zone, which is what allows the subsequent `drop` event (handled in Blazor) to fire.
(function () {
    const DROP_ZONES = '.col-cell, .row-drop-zone, .empty-canvas';

    function overDropZone(target) {
        return target && target.closest && target.closest(DROP_ZONES) !== null;
    }

    function allowDrop(e) {
        if (overDropZone(e.target)) {
            e.preventDefault();
        }
    }

    document.addEventListener('dragenter', allowDrop, false);
    document.addEventListener('dragover', allowDrop, false);
})();
