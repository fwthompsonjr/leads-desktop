var indx = '{0}';
var startDate = '{1}';
var endDate = '{2}';
var searchDate = '{3}';
var fallbackDate = new Date().toLocaleString().split(',')[0];
var search =
{
	setLocation: function(id) {
		const cboname = "cboHSLocationGroup";
		var cbo = document.getElementById(cboname);
		cbo.selectedIndex = id;
		cbo.dispatchEvent(new Event('change'))
	},
	setSearchType: function() {
		const cboname = "cboHSSearchBy";
		var cbo = document.getElementById(cboname);
		cbo.selectedIndex = 1;
		cbo.dispatchEvent(new Event('change'))
	},
	setSearchValue: function(id, dteStart)
	{
		const tbxname = "SearchCriteria_SearchValue";
		var tbx = document.getElementById(tbxname);
		var tbxvalue = "";
		var dte = (new Date(dteStart)).getFullYear().toString();
		if (id == 2) { tbxvalue = "".concat(dte, "CV*"); }
		if (id == 3) { tbxvalue = "".concat(dte, "CI*"); }
		if (id == 4) { tbxvalue = "11DC2*"; }
		tbx.value = tbxvalue;
		tbx.dispatchEvent(new Event('change'))	
	},
	setSearchDates: function(dteStart, dteEnding)
	{
		const tbxnames = "SearchCriteria_DateFrom,SearchCriteria_DateTo";
		var tbxs = tbxnames.split(',');
		for(var i = 0; i < tbxs.length; i++) {
			var tbxname = tbxs[i];
			var tbx = document.getElementById(tbxname);
			var tbxvalue = i == 0 ? dteStart : dteEnding;
			tbx.value = tbxvalue;
			tbx.dispatchEvent(new Event('change'))
		}	
	},
	populate: function(id, dteStart, dteEnding, dteSearch)
	{
		if (isNaN(id)) { return; }
		if (isNaN(Date.parse(dteStart))) { return; }
		if (isNaN(Date.parse(dteEnding))) { return; }
		if (isNaN(Date.parse(dteSearch))) { 
			dteSearch = fallbackDate;
		}
		nbr = parseInt(id);
		search.setLocation(0);
		search.setLocation(nbr);
		search.setSearchType();
		search.setSearchValue(nbr, dteSearch);
		search.setSearchDates(dteStart, dteEnding);
		search.submit();
	},
	submit: function()
	{
		const btnname = "btnHSSubmit";
		var btn = document.getElementById(btnname);
		btn.click();
	}
}
// if searchDate is not provided assume search in current year
if (isNaN(Date.parse(searchDate))) { 
	searchDate = fallbackDate;
}
search.populate(indx, startDate, endDate, searchDate);