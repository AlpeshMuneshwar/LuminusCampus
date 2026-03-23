document.addEventListener("DOMContentLoaded", function () {
  const sidebar = document.querySelector("#sidebar");
  const logo = document.querySelector(".sidebar-logo");
  const overlay = document.createElement("div");
  const toggleBtn = document.querySelector("#sidebar-toggle-btn");

  overlay.className = "sidebar-overlay";
  document.body.appendChild(overlay);

  // Toggle Overlay Helper
  function toggleOverlay() {
    if (window.innerWidth <= 768) {
      if (sidebar.classList.contains("mobile-open")) {
        overlay.classList.add("show");
      } else {
        overlay.classList.remove("show");
      }
    }
  }

  // 1. Logo Click (Desktop: Collapse / Mobile: Close)
  if (logo) {
    logo.addEventListener("click", function (e) {
      if (window.innerWidth > 768) {
        // Desktop: Toggle Expand
        sidebar.classList.toggle("expand");
      } else {
        // Mobile: Toggle Open/Close
        sidebar.classList.toggle("mobile-open");
        toggleOverlay();
      }
      e.stopPropagation();
    });
  }

  // 2. Mobile Hamburger Toggle
  if (toggleBtn) {
    toggleBtn.addEventListener("click", function (e) {
      sidebar.classList.toggle("mobile-open");
      toggleOverlay();
      e.stopPropagation();
    });
  }

  // 3. Close Sidebar when clicking outside (Mobile)
  document.addEventListener("click", function (e) {
    if (window.innerWidth <= 768) {
      if (sidebar.classList.contains("mobile-open") &&
        !sidebar.contains(e.target) &&
        !toggleBtn.contains(e.target)) {

        sidebar.classList.remove("mobile-open");
        toggleOverlay();
      }
    }
  });

  // 4. Close Sidebar when a link is clicked (Mobile)
  const links = document.querySelectorAll(".sidebar-link:not(.has-dropdown)");
  links.forEach(link => {
    link.addEventListener("click", function () {
      if (window.innerWidth <= 768) {
        sidebar.classList.remove("mobile-open");
        toggleOverlay();
      }
    });
  });

  // 5. Handle Window Resize
  window.addEventListener("resize", function () {
    if (window.innerWidth > 768) {
      overlay.classList.remove("show");
      sidebar.classList.remove("mobile-open"); // Ensure mobile class is gone
      // We do NOT add/remove 'collapsed' here, preserving user state
    }
  });
});
