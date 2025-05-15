let execution_mode = '{0}';
let pager_status = {
 'name': 'Get paging detail',
 'result': true,
 'hasNextPage': false
}
try {
	var place_holder_name = 'ctl00_ContentPlaceHolder1_DataPagerLisViewCases1'
	var place_holder = document.getElementById(place_holder_name);
	var links = Array.prototype.slice.call( place_holder.getElementsByTagName('a'), 0 );
	var next_pg = links.find(x => x.innerText.trim().toLowerCase() == 'next' && x.getAttribute('disabled') == null);
	pager_status.hasNextPage = next_pg != null;
	if (execution_mode != 'execute') { return JSON.stringify(pager_status); }
	next_pg.click();
} catch {
	pager_status.result = false;
	return JSON.stringify(pager_status);
}