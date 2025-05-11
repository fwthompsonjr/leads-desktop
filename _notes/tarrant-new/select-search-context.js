let populate_search_context = {
    'search_type': '0',
    'location_name': 'All JP Courts',
    'linkitems': function () {
        const cs_search = 'Case Records Search'
        const ms_search = 'Class C Misdemeanors Only';
        return [
            cs_search.toLowerCase(),
            ms_search.toLowerCase()
        ]
    },
    'get_hyperlinks': function () {
        let linkitems = populate_search_context.linkitems();
        let links = Array.prototype.slice.call(document.getElementsByTagName('a'), 0);
        links = links.filter(n => { return n.classList.contains('ssSearchHyperlink') });
        links = links.filter(n => {
            let txt = String(n.innerText).trim().toLowerCase();
            return (txt == linkitems[0] || txt == linkitems[1]);
        });
        links = links.filter(n => {
            let txt = String(n.innerText).trim().toLowerCase();
            return (txt == linkitems[0] || txt == linkitems[1]);
        });
        return links;
    },
    'set_location': function () {
        const change_event = new Event('change', { bubbles: true }); 
        let location_name = populate_search_context.location_name.trim().toLowerCase();
        let cbo = document.getElementById('sbxControlID2');
        let opts = Array.prototype.slice.call(cbo.getElementsByTagName('option'), 0);
        let optId = opts.findIndex(x => x.innerText.trim().toLowerCase() == location_name);
        cbo.selectedIndex = optId == -1 ? 1 : optId;
        cbo.dispatchEvent(change_event);
    },
    'can_navigate': function () {
        let search_type = populate_search_context.search_type;
        if (search_type != '0' && search_type != '1') return false;
        let links = populate_search_context.get_hyperlinks();
        return links.length > 0;
    },
    navigate: function () {
        let canNavigate = populate_search_context.can_navigate();
        if (!canNavigate) {
            throw new Error('Error invalid parameter defintion.');
        }
        let searchId = parseInt(populate_search_context.search_type);
        let linkitems = populate_search_context.linkitems();
        let links = populate_search_context.get_hyperlinks();
        let lnk = links.find(x => x.innerText.trim().toLowerCase() == linkitems[searchId]);
        populate_search_context.set_location();
        lnk.click();
    }
}

let parm_search_type = '{0}'; // default to case records search
let parm_location_name = '{1}'
populate_search_context.search_type = parm_search_type;
populate_search_context.location_name = parm_location_name;
populate_search_context.navigate();