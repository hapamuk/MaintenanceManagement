(() => {
  'use strict';
  const root = document.getElementById('ac-detail-tabs');
  if (!root) return;
  const tabs = [...root.querySelectorAll('button[data-tab]')];
  const panes = [...root.querySelectorAll('section[data-pane]')];
  function setTab(tab, focus = false) {
    tabs.forEach(t => {
      const active = t === tab;
      t.classList.toggle('active', active);
      t.setAttribute('aria-selected', String(active));
      t.tabIndex = active ? 0 : -1;
    });
    panes.forEach(p => { p.hidden = p.dataset.pane !== tab.dataset.tab; });
    if (focus) tab.focus();
  }
  tabs.forEach((tab, idx) => {
    tab.addEventListener('click', () => setTab(tab));
    tab.addEventListener('keydown', e => {
      if (!['ArrowRight','ArrowLeft','Home','End'].includes(e.key)) return;
      e.preventDefault();
      const next = e.key === 'Home' ? 0 : e.key === 'End' ? tabs.length - 1
        : (idx + (e.key === 'ArrowRight' ? 1 : -1) + tabs.length) % tabs.length;
      setTab(tabs[next], true);
    });
  });
  setTab(root.querySelector('[data-tab="info"]') || tabs[0]);
  const type = document.getElementById('ac-history-type');
  const start = document.getElementById('adv-history-start');
  const end = document.getElementById('adv-history-end');
  const button = document.getElementById('adv-history-filter');
  const rows = [...root.querySelectorAll('tr[data-history-type]')];
  const counter = document.getElementById('ac-history-count');
  const empty = document.getElementById('adv-history-empty');
  function filter() {
    const kind = type?.value || '';
    const from = start?.value || '';
    const to = end?.value || '';
    if (from && to && from > to) { start.setCustomValidity('Başlangıç tarihi bitiş tarihinden sonra olamaz.'); start.reportValidity(); return; }
    start?.setCustomValidity('');
    let shown = 0;
    rows.forEach(row => {
      const date = row.dataset.historyDate || '';
      const visible = (!kind || kind === row.dataset.historyType) && (!from || date >= from) && (!to || date <= to);
      row.hidden = !visible;
      if (visible) shown++;
    });
    if (counter) counter.textContent = `${shown} / ${rows.length} hareket gösteriliyor`;
    if (empty) empty.hidden = shown !== 0 || rows.length === 0;
  }
  button?.addEventListener('click', filter);
  type?.addEventListener('change', filter);
  [start, end].forEach(el => el?.addEventListener('change', () => el.setCustomValidity('')));
})();
