let dvs = Array.prototype.slice.call(document.getElementsByTagName('div'), 0);
dvs = dvs.filter(d => d.classList.contains('ssCaseDetailSectionTitle'));
let party_info = dvs.find(x => x.innerText.trim() == 'Party Information')
let party_table = party_info.closest('table');
var table_rows = Array.prototype.slice.call(party_table.getElementsByTagName('tr'), 0);
table_rows = table_rows.filter(x => x.innerHTML.indexOf('class="ssTableHeader"') > 0 && x.innerText.indexOf('Lead Attorneys') == -1);

function get_case_style() {
    tbls = Array.prototype.slice.call(document.getElementsByTagName('table'), 0)
    tbls = tbls.filter(t => {
        let dta = t.innerText;
        if (dta.indexOf('Case Type:') < 0) { return false; }
        if (dta.indexOf('Date Filed:') < 0) { return false; }
        if (dta.indexOf('Location:') < 0) { return false; }
        return true;

    });
    if (tbls.length == 0) return '';
    let case_table = tbls[0];
    if (case_table.rows.length == 0) return '';
    let case_table_row = case_table.rows[0];
    if (case_table_row.cells.length == 0) return '';
    return case_table_row.cells[0].innerText;
}
function get_address(tr) {
    const nbs = '&nbsp;';
    const br = '<br>';
    const ppe = '|';
    const dbppe = '||';
    try {
        let parent_table = tr.closest('table');
        let row_id = tr.rowIndex;
        let addr_id = row_id + 1;
        if (addr_id > parent_table.rows.length - 1) {
            return '';
        }
        let html = String(parent_table.rows[addr_id].cells[0].innerHTML);
        while (html.indexOf(nbs) >= 0) { html = html.replace(nbs, ' ').trim(); }
        while (html.indexOf(br) >= 0) { html = html.replace(br, ppe).trim(); }
        while (html.indexOf(dbppe) >= 0) { html = html.replace(dbppe, ppe).trim(); }
        return html;
    } catch {
        return '';
    }

}
let caseStyle = get_case_style();
let people = [];
for (let tr in table_rows) {
    let person = {
        "type": tr.cells[0].innerText.trim(),
        "name": tr.cells[1].innerText.trim(),
        "address": get_address(tr)
    }
    people.push(person);
}

return JSON.stringify(people);
