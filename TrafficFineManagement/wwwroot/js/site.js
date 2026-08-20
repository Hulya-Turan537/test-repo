document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("rejectFineForm");
    if (!form) {
        return;
    }

    const reason = form.querySelector("#rejectReason");
    const error = form.querySelector("#rejectReasonError");
    const modal = document.getElementById("rejectModal");

    const clearError = () => {
        error?.classList.add("d-none");
        reason?.classList.remove("is-invalid");
    };

    form.addEventListener("submit", (event) => {
        const value = (reason?.value || "").trim();
        if (value.length < 3) {
            event.preventDefault();
            reason?.classList.add("is-invalid");
            if (error) {
                error.textContent = "Ret nedeni en az 3 karakter olmalıdır.";
                error.classList.remove("d-none");
                error.classList.add("d-block");
            }
        }
    });

    reason?.addEventListener("input", clearError);
    modal?.addEventListener("hidden.bs.modal", () => {
        form.reset();
        clearError();
        error?.classList.remove("d-block");
    });
});
