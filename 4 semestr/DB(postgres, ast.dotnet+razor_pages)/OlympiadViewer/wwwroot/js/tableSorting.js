document.addEventListener("DOMContentLoaded", function () {

    const sortableHeaders =
        document.querySelectorAll(".sortable");

    sortableHeaders.forEach(header => {

        header.addEventListener("click", function () {

            const column =
                this.dataset.column;

            let currentDirection =
                this.dataset.direction;

            if (currentDirection === "asc") {
                currentDirection = "desc";
            }
            else {
                currentDirection = "asc";
            }

            this.dataset.direction =
                currentDirection;

            const currentUrl =
                new URL(window.location.href);

            currentUrl.searchParams.set(
                "sortColumn",
                column);

            currentUrl.searchParams.set(
                "sortDirection",
                currentDirection);

            window.location.href =
                currentUrl.toString();
        });
    });
});