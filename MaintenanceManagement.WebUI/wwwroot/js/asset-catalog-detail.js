(() => {
  "use strict";
  const root = document.getElementById('ac-detail-tabs');
  if (!root) return;
  const tabs = [...root.querySelectorAll('[data-tab]')];
  const panes = [...root.querySelectorAll('[data-pane]')];
  function selectTab(tab) {
    tabs.forEach(t => {
      const selected = t === tab;
      t.classList.toggle('active', selected);
      t.setAttribute('aria-selected', String(selected));
      t.tabIndex = selected ? 0 : -1;
    });
    panes.forEach(p => { p.hidden = p.dataset.pane !== tab.dataset.tab; });
  }
  tabs.forEach((tab, index) => {
    tab.addEventListener('click', () => selectTab(tab));
    tab.addEventListener('keydown', event => {
      if (!['ArrowLeft','ArrowRight','Home','End'].includes(event.key)) return;
      event.preventDefault();
      const next = event.key === 'Home' ? 0 : event.key === 'End' ? tabs.length - 1
        : (index + (event.key === 'ArrowRight' ? 1 : -1) + tabs.length) % tabs.length;
      selectTab(tabs[next]); tabs[next].focus();
    });
  });
  const type = document.getElementById('ac-history-type');
  const search = document.getElementById('ac-history-search');
  const counter = document.getElementById('ac-history-count');
  const rows = [...root.querySelectorAll('tr[data-history-type]')];
  function applyHistoryFilters() {
    const kind = type?.value || '';
    const term = (search?.value || '').trim().toLocaleLowerCase('tr-TR');
    let visible = 0;
    rows.forEach(row => {
      const matches = (!kind || row.dataset.historyType === kind) &&
        (!term || row.textContent.toLocaleLowerCase('tr-TR').includes(term));
      row.hidden = !matches;
      if (matches) visible++;
    });
    if (counter) counter.textContent = `${visible} / ${rows.length} hareket gösteriliyor`;
  }
  type?.addEventListener('change', applyHistoryFilters);
  search?.addEventListener('input', applyHistoryFilters);
})();
