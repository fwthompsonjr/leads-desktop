﻿using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    public partial class FormMain
    {

        private void Button1_Click(object sender, EventArgs e)
        {
            string driverNames = string.Concat("geckodriver,", CommonKeyIndexes.ChromeDriver);
            try
            {
                KillProcess(driverNames);
                SetStatus(StatusType.Running);
                using var hider = new HideProcessWindowHelper();
                if (!ValidateCustom())
                {
                    SetStatus(StatusType.Ready);
                    return;
                }
                txConsole.Text = string.Empty;
                ProcessStartingMessage();
                var startDate = dteStart.Value.Date;
                var endingDate = dteEnding.Value.Date;
                var siteData = (WebNavigationParameter)cboWebsite.SelectedItem;
                var sourceMember = (SourceType)siteData.Id;
                CurrentRequest = sourceMember.GetDbRequest(this, startDate);
                var cbindx = GetCaseSelectionIndex(cboCaseType.SelectedItem);
                var websiteName = siteData.Name;
                if (cbindx == 0 && siteData.Id == 30)
                {
                    websiteName = "Harris Criminal Courts";
                }
                var searchItem = new SearchResult
                {
                    Id = GetObject<List<SearchResult>>(Tag).Count + 1,
                    Website = websiteName,
                    EndDate = endingDate.ToShortDateString(),
                    StartDate = startDate.ToShortDateString(),
                    SearchDate = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString(),
                };
                searchItem.Search = $"{searchItem.SearchDate} : {websiteName} from {searchItem.StartDate} to {searchItem.EndDate}";
                switch (siteData.Id)
                {
                    case (int)SourceType.BexarCounty:
                        BexarButtonExecution(siteData, searchItem);
                        break;
                    case (int)SourceType.DallasCounty:
                        DallasButtonExecution(siteData, searchItem);
                        break;
                    case (int)SourceType.TravisCounty:
                        TravisButtonExecution(siteData, searchItem, startDate, endingDate);
                        break;
                    case (int)SourceType.HarrisCivil:
                        CommonButtonExecution(siteData, searchItem, cbindx);
                        break;
                    case (int)SourceType.HidalgoCounty:
                    case (int)SourceType.ElPasoCounty:
                    case (int)SourceType.FortBendCounty:
                    case (int)SourceType.WilliamsonCounty:
                    case (int)SourceType.GraysonCounty:
                    case (int)SourceType.TarrantCounty:
                        CommonButtonExecution(siteData, searchItem);
                        break;
                    default:
                        NonDallasButtonExecution(siteData, searchItem, startDate, endingDate);
                        break;
                }

            }
            catch (Exception ex)
            {
                SetStatus(StatusType.Error);
                Console.WriteLine(CommonKeyIndexes.UnexpectedErrorOccurred);
                Console.WriteLine(ex.Message);

                Console.WriteLine(CommonKeyIndexes.DashedLine);
            }
            finally
            {
                KillProcess(driverNames);
                CurrentRequest = null;
            }
        }

        private void CommonButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, int caseSelectionIndex = 0)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            if (siteData.Id == 10)
            {
                TarrantSetUserSelections(keys);
            }
            var wb = new WebNavigationParameter { Keys = keys };
            int calculcatedSiteIndex = siteData.Id;
            if (calculcatedSiteIndex == 30 && caseSelectionIndex != 0 )
            {
                calculcatedSiteIndex = 35;
            }

            IWebInteractive dweb = calculcatedSiteIndex switch
            {
                10 => new TarrantRvUiInteractive(wb),
                30 => new HccUiInteractive(wb),
                35 => new HarrisRvInteractive(wb),
                130 => new GraysonUiInteractive(wb),
                120 => new WilliamsonUiInteractive(wb),
                110 => new FortBendUiInteractive(wb),
                100 => new ElPasoUiInteractive(wb),
                _ => new HidalgoUiInteractive(wb)
            };
            var indx = calculcatedSiteIndex == 30 ? 1 : 0;
            _ = Task.Run(async () =>
            {
                await Invoke(async () =>
                {
                    await ProcessAsync(dweb, siteData, searchItem, indx);
                });

            }).ConfigureAwait(true);
        }

        private void TarrantSetUserSelections(List<WebNavigationKey> keys)
        {
            var courtName = Invoke(() =>
            {
                var idx = cboCourts.SelectedIndex;
                var fallback = new TarrantUserOption();
                if (idx < 0) return fallback;
                if (cboCourts.DataSource is List<Option> list)
                {
                    fallback.Index = idx;
                    fallback.Name = list[idx].Name;
                    return fallback;
                }
                else
                {
                    return fallback;
                }
            });
            var searchMode = Invoke(() =>
            {
                var isCriminalSearch = chkCrimalCases.Visible && chkCrimalCases.Checked;
                var fallback = "Civil";
                return isCriminalSearch ? "Criminal" : fallback;

            });
            keys.Add(new() { Name = "UserSelectedCourtIndex", Value = $"{courtName.Index}" });
            keys.Add(new() { Name = "UserSelectedCourtType", Value = courtName.Name });
            keys.Add(new() { Name = "UserSelectedSearchName", Value = searchMode });
        }

        private void BexarButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            var dweb = new BexarUiInteractive(wb);
            _ = Task.Run(async () =>
            {
                await Invoke(async () =>
                {
                    await ProcessAsync(dweb, siteData, searchItem);
                });

            }).ConfigureAwait(true);
        }

        private void DallasButtonExecution(WebNavigationParameter siteData, SearchResult searchItem)
        {
            var container = ActionSettingContainer.GetContainer;
            var index = cboSearchType.SelectedIndex;
            var searchType = DallasSearchProcess.GetCourtName(index);
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = searchItem.StartDate},
                new() { Name = "EndDate", Value = searchItem.EndDate },
                new() { Name = "CourtType", Value = searchType }
            };
            var wb = new WebNavigationParameter { Keys = keys };
            var dweb = new DallasUiInteractive(wb);
            var dbweb = new UiDbInteractive(
                dweb,
                container.GetInstance<IRemoteDbHelper>(),
                CurrentRequest);
            _ = Task.Run(async () =>
            {
                await Invoke(async () =>
                {
                    await ProcessAsync(dbweb, siteData, searchItem);
                });
            }).ConfigureAwait(true);
        }

        private void TravisButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, DateTime startDate, DateTime endingDate)
        {
            var dto = (DropDown)cboSearchType.SelectedItem;
            var txt = dto.Name.Split(' ')[0];
            var searchType = txt.ToUpper(CultureInfo.CurrentCulture).Trim();
            var search = new TravisSearchProcess();
            search.Search(startDate, endingDate, searchType);
            var dweb = search.GetUiInteractive();
            _ = Task.Run(async () =>
            {
                await Invoke(async () =>
                {
                    await ProcessAsync(dweb, siteData, searchItem);
                });
            }).ConfigureAwait(true);
        }

        private void NonDallasButtonExecution(WebNavigationParameter siteData, SearchResult searchItem, DateTime startDate, DateTime endingDate)
        {
            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            var isDentonCounty = siteData.Id == (int)SourceType.DentonCounty;
            var keys = siteData.Keys;
            var isDistrictSearch = keys.Find(x =>
                x.Name.Equals(CommonKeyIndexes.DistrictSearchType,
                ccic)) != null;
            var criminalToggle = keys.Find(x =>
                x.Name.Equals(CommonKeyIndexes.CriminalCaseInclusion,
                ccic));
            if (isDentonCounty && criminalToggle != null)
            {
                criminalToggle.Value = isDistrictSearch ?
                    CommonKeyIndexes.NumberZero :
                    CommonKeyIndexes.NumberOne;
            }

            if (!isDentonCounty && criminalToggle != null)
            {
                criminalToggle.Value = CommonKeyIndexes.NumberOne;
            }
            var collinIndex = (int)SourceType.CollinCounty;
            if (siteData.Id == collinIndex)
            {
                siteData.Keys.Add(new() { Name = "StartDate", Value = $"{startDate:d}" });
                siteData.Keys.Add(new() { Name = "EndDate", Value = $"{endingDate:d}" });
                siteData.Keys.Add(new() { Name = "CourtType", Value = "Civil" });
            }
            IWebInteractive webmgr =
            siteData.Id == collinIndex ?
            new CollinUiInterative(siteData, startDate, endingDate) :
            WebFetchingProvider.GetInteractive(siteData, startDate, endingDate);
            var cbindx = siteData.Id != 30 ? 0 : GetCaseSelectionIndex(cboCaseType.SelectedItem);
            _ = Task.Run(async () =>
            {
                await Invoke(async () =>
                {
                    await ProcessAsync(webmgr, siteData, searchItem, cbindx);
                });

            }).ConfigureAwait(true);
        }


        private IWebInteractive GetDbInteractive(IWebInteractive interactive, int siteId)
        {
            if (interactive is UiDbInteractive db) return db;
            if (interactive is HccUiInteractive hcc) return hcc;
            var container = ActionSettingContainer.GetContainer;
            if (CurrentRequest == null)
            {
                var member = (SourceType)siteId;
                CurrentRequest = member.GetDbRequest(this, DateTime.Now.Date);
            }
            var response = new UiDbInteractive(
                interactive,
                container.GetInstance<IRemoteDbHelper>(),
                CurrentRequest)
            {
                ReportProgress = TryShowProgress,
                ReportProgessComplete = TryHideProgress,
                StartDate = dteStart.Value.Date,
                EndingDate = dteEnding.Value.Date,
                TrackingIndex = interactive.TrackingIndex,
                Parameters = interactive.Parameters ?? new()
            };
            return response;
        }

        private async Task ProcessAsync(
            IWebInteractive webmgr,
            WebNavigationParameter siteData,
            SearchResult searchItem,
            int caseSelectionIndex = 0)
        {
            var tracking = new
            {
                CountyId = siteData.Id,
                StartDate = dteStart.Value.Date,
                EndDate = dteEnding.Value.Date
            };
            var trackingItem = dbHelper.AppendUsage(tracking.CountyId, tracking.StartDate, tracking.EndDate);
            var context = new SearchContext { Id = trackingItem.Id };
            var isAdmin = IsAccountAdmin();
            OnSearchProcessBegin(context);
            try
            {
                if (isAdmin)
                {
                    var displayMode = SettingsWriter.GetSettingOrDefault("admin", SettingConstants.AdminFieldNames.AllowBrowserDisplay, true);
                    if (!displayMode) { webmgr.DriverReadHeadless = false; }
                }
                if (siteData.Id == (int)SourceType.TarrantCounty)
                {
                    webmgr.DriverReadHeadless = false;
                }
                webmgr.TrackingIndex = trackingItem.Id;
                webmgr.ReportProgress = TryShowProgress;
                webmgr.ReportProgessComplete = TryHideProgress;
                var interactive = GetDbInteractive(webmgr, siteData.Id);
                interactive.TrackingIndex = trackingItem.Id;
                using var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                CaseData = await Task.Run(() =>
                {
                    if (token.IsCancellationRequested) return new();
                    return interactive.Fetch(token);
                }, token).ConfigureAwait(true);

                var nonactors = new List<int> {
                    (int)SourceType.DallasCounty,
                    (int)SourceType.TravisCounty,
                    (int)SourceType.BexarCounty,
                    (int)SourceType.HarrisCivil,
                    (int)SourceType.HidalgoCounty,
                    (int)SourceType.ElPasoCounty,
                    (int)SourceType.FortBendCounty,
                    (int)SourceType.WilliamsonCounty,
                    (int)SourceType.GraysonCounty,
                    (int)SourceType.TarrantCounty,
                };

                ProcessEndingMessage();
                SetStatus(StatusType.Finished);
                KillProcess(CommonKeyIndexes.ChromeDriver);
                if (CaseData == null)
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                if (string.IsNullOrEmpty(CaseData.Result))
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                if (IsEmpty(CaseData))
                {
                    throw new KeyNotFoundException(CommonKeyIndexes.NoDataFoundFromCaseExtract);
                }
                CaseData.WebsiteId = siteData.Id;
                // write search count to api
                var addresses = CaseData.PeopleList;
                if (addresses != null && addresses.Count > 0)
                {
                    addresses.RemoveAll(x => string.IsNullOrWhiteSpace(x.DateFiled) || string.IsNullOrWhiteSpace(x.Court));
                    addresses.ForEach(a =>
                    {
                        if (DateTime.TryParse(a.DateFiled, CultureInfo.CurrentCulture.DateTimeFormat, out var date))
                        {
                            a.DateFiled = $"{date:d}";
                        }
                    });
                    CaseData.PeopleList = addresses;
                }
                var count = CaseData.PeopleList.Count;
                if (count > 0)
                {
                    var adjustedCount = CaseData.AdjustedRecordCount == 0 ? count : CaseData.AdjustedRecordCount;
                    dbHelper.CompleteUsage(trackingItem.Id, adjustedCount, GetShortName(CaseData));
                    var member = (SourceType)siteData.Id;
                    var userName = GetUserName();
                    var searchRange = $"{webmgr.StartDate:d} to {webmgr.EndingDate:d}";
                    if (string.IsNullOrWhiteSpace(userName)) { userName = "unknown"; }
                    var countyName = member.GetCountyName();
                    UsageIncrementer.IncrementUsage(userName, countyName, adjustedCount, searchRange);
                    UsageReader.WriteUserRecord();
                }
                var isHarrisCriminal = caseSelectionIndex == 1 && siteData.Id == (int)SourceType.HarrisCivil;
                if (!isHarrisCriminal && !nonactors.Contains(siteData.Id))
                {
                    ExcelWriter.WriteToExcel(CaseData);
                }
                searchItem.ResultFileName = CaseData.Result;
                searchItem.AddressList = CaseData.PeopleList.ConvertFrom();
                searchItem.IsCompleted = true;
                context.LocalFileName = searchItem.ResultFileName;
                context.FileStatus = isAdmin ? "DECODED" : "ENCODED";
                OnSearchProcessComplete(context);
                searchItem.MoveToCommon();
                GetObject<List<SearchResult>>(Tag).Add(searchItem);
                ComboBox_DataSourceChanged(null, null);
                var button = ButtonDentonSetting;
                var txt = $"{button.Text};Open File";
                Invoke(() =>
                {
                    button.Tag = txt;
                    button.Visible = true;
                });
                TryOpenExcel();
                SetStatus(StatusType.Ready);
            }
            catch (Exception ex)
            {
                if (IsOfflineProcess(ex, tracking.CountyId))
                {
                    SetStatus(StatusType.Ready);
                    return;
                }
                SetStatus(StatusType.Error);
                Console.WriteLine(CommonKeyIndexes.UnexpectedErrorOccurred);
                Console.WriteLine(ex.Message);

                Console.WriteLine(CommonKeyIndexes.DashedLine);
            }
            finally
            {
                KillProcess(CommonKeyIndexes.ChromeDriver);
                KillProcess("geckodriver");
                ClearProgressDate();
                TryHideProgress();
            }
        }

        private static bool IsOfflineProcess(Exception ex, int countyId = 0)
        {
            var offlineIndexes = new List<int> {
                    (int)SourceType.DallasCounty,
                };
            if (ex is not KeyNotFoundException) return false;
            if (!offlineIndexes.Contains(countyId)) return false;
            return SettingsWriter.GetSettingOrDefault(
                "search",
                SettingConstants.SearchFieldNames.AllowOfflineProcessing,
                true);
        }

        private static string GetShortName(WebFetchResult web)
        {
            var result = web.Result;
            if (string.IsNullOrWhiteSpace(result)) return string.Empty;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(result);
            var extn = System.IO.Path.GetExtension(result);
            if (string.IsNullOrWhiteSpace(fileName)) return result;
            if (string.IsNullOrWhiteSpace(extn)) extn = string.Empty;
            return string.Concat(fileName, extn);
        }

        private static readonly IRemoteDbHelper dbHelper
            = ActionSettingContainer.GetContainer.GetInstance<IRemoteDbHelper>();
        private sealed class TarrantUserOption
        {
            public int Index { get; set; } = 1;
            public string Name { get; set; } = "All JP Courts";
        }
    }
}
