(() => {
  const tabs=[...document.querySelectorAll('[data-tab]')];
  const panes=[...document.querySelectorAll('[data-pane]')];
  tabs.forEach(tab=>tab.addEventListener('click',()=>{
    tabs.forEach(t=>t.classList.toggle('active',t===tab));
    panes.forEach(p=>p.hidden=p.dataset.pane!==tab.dataset.tab);
  }));
  const type=document.getElementById('aw-history-type');
  const search=document.getElementById('aw-history-search');
  function filter(){
    document.querySelectorAll('tr[data-history-type]').forEach(row=>{
      row.hidden=!!(type.value && type.value!==row.dataset.historyType) || !row.textContent.toLocaleLowerCase('tr-TR').includes(search.value.toLocaleLowerCase('tr-TR'));
    });
  }
  type?.addEventListener('change',filter); search?.addEventListener('input',filter);
})();
