/* Admin V8.7: Yalnizca ortak kabuk icin, sayfa scriptlerine dokunmaz. */
(function () {
  'use strict';
  function init() {
    if (!document.body.classList.contains('admin-unified-premium')) return;
    var nav = document.getElementById('accordionSidebar');
    if (!nav) return;
    // Bootstrap'in kendi collapse mekanizmasini kullanir; eventleri yeniden baglamaz.
    if (window.jQuery && window.jQuery.fn && window.jQuery.fn.collapse) {
      window.jQuery(nav).on('shown.bs.collapse hidden.bs.collapse', function (event) {
        var el = event.target;
        if (!el || !el.id) return;
        var toggle = nav.querySelector('[data-target="#' + el.id + '"]');
        if (toggle) toggle.setAttribute('aria-expanded', el.classList.contains('show') ? 'true' : 'false');
      });
    }
    // Yalnizca ortak gorunum icin bir sinif; fonksiyonel islemlere mudahale etmez.
    document.documentElement.classList.add('admin-theme-ready');
  }
  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init, {once:true});
  else init();
})();
