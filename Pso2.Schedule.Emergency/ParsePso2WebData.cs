using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Pso2.Schedule.Emergency
{
	public class ParsePso2WebData
	{
		internal static readonly Uri PSO2ScheduleWebSiteAddress = new Uri("http://pso2.jp/players/news/?charid=i_boostevent"); //site address
		internal static readonly Regex DayRegexPattern = new Regex("[1-2]*[0-9]+月[1-3]*[0-9]*日");
		internal static readonly TimeSpan DefaultEmergencyTime = new TimeSpan(0, 30, 0); //30minute default
		internal static readonly int DefaultMaintenanceEndTime = 17; //PM17:00 メンテ終了
		internal string HTMLData {
            get;
            set;
        }

        public EmergencyDataFormatCollection ImportFromWebSite() {
            EmergencyDataFormatCollection result = new EmergencyDataFormatCollection();
            try {
                BaseMessage pso2WebSiteData = this.GetPSO2WebSiteData();
                if (pso2WebSiteData.Status != ResponceStatus.OK) {
                    result.SetResponceData(pso2WebSiteData);
                    return result;
                }
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pso2WebSiteData.GetData.Replace("\r\n", ""));

                HtmlNodeCollection mainData = htmlDocument.DocumentNode.SelectNodes("//div[@class='tableWrap']");
                if (mainData == null || mainData.Nodes().Count() == 0) {
                    result.SetResponceData(null, ResponceStatus.FormatChanged, new Exception(), "FormatChanged");
                    return result;
                }
                foreach (HtmlNode informationNode in mainData) {
                    HtmlNode dayNode = informationNode.PreviousSibling;
                    EmergencyDataFormat emergencyDataFormat = new EmergencyDataFormat();
                    if (dayNode == null) {

                        continue;
                    }
                    DateTime? day = this.GetDay(dayNode);

                    if (day == null) {
                        //日情報なし
                        continue;
                    }
                    emergencyDataFormat.SetDay(day.Value);
                    this.SetInformation(informationNode, emergencyDataFormat, result);
                    
                }
            } catch (Exception ex) {
                result.SetResponceData(null, ResponceStatus.UnknownError, ex, "Unknown Error");
            }
            return result;
        }
        /// <summary>
        /// pso2のWebサイトからDLする
        /// </summary>
        /// <returns></returns>
		internal BaseMessage GetPSO2WebSiteData() {
            BaseMessage result = new BaseMessage();
            try {
                WebRequest webRequest = WebRequest.CreateHttp(ParsePso2WebData.PSO2ScheduleWebSiteAddress);
                WebResponse response = webRequest.GetResponse();
                string text;
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream())) {
                    text = streamReader.ReadToEnd();
                    this.HTMLData = text;
                }
                result.SetResponceData(text);
            } catch (WebException ex) {
                result.SetResponceData(null, ResponceStatus.NetworkError, ex, "GetPSO2WebSiteData Error");
            }
            return result;
        }
        /// <summary>
        /// ResultDataに結果を格納する
        /// </summary>
        /// <param name="InformationNode"></param>
        /// <param name="BaseEmergencyData"></param>
        /// <param name="ResultData"></param>
		internal BaseMessage SetInformation(HtmlNode InformationNode, EmergencyDataFormat BaseEmergencyData, EmergencyDataFormatCollection ResultData) {
            BaseMessage result = new BaseMessage();
            
            foreach (HtmlNode current in InformationNode.ChildNodes) {
                for (HtmlNode htmlNode = current.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling) {
                    //ヘッダーは無視
                    if (htmlNode == current.FirstChild) {
                        continue;
                    }
                    EmergencyDataFormat emergencyData = (EmergencyDataFormat)BaseEmergencyData.Clone();

                    for (int columnCount = 0; columnCount < 3; columnCount++) {
                        if (htmlNode.ChildNodes.Count != 3) {
                            //unknown format
                            result.SetResponceData(null, ResponceStatus.FormatChanged, new Exception(), "SetInfomation:TableNode Not 3");
                            continue;
                        }
                        switch (columnCount) {
                            case 0:
                                //時間
                                string eventTimeText = htmlNode.ChildNodes[columnCount].InnerText;
                                KeyValuePair<TimeSpan, TimeSpan> time = this.GetTime(eventTimeText);
                                emergencyData.StartTime = emergencyData.StartTime.Add(time.Key);
                                emergencyData.EndTime = emergencyData.EndTime.Add(time.Value);
                                break;

                            case 1:
                                //種類
                                string eventTypeText = htmlNode.ChildNodes[columnCount].FirstChild.Attributes["alt"].Value;
                                emergencyData.EventType = this.GetEventType(eventTypeText);
                                break;

                            case 2:
                                //イベント内容
                                string eventInfoText = htmlNode.ChildNodes[columnCount].InnerText;
                                emergencyData.EventInformation = eventInfoText;
                                break;
                        }
                    }
                    ResultData.Add(emergencyData);
                }
            }
            return result;
        }
        /// <summary>
        /// 緊急の時間
        /// </summary>
        /// <param name="TimeValue"></param>
        /// <returns></returns>
		internal KeyValuePair<TimeSpan, TimeSpan> GetTime(string TimeValue) {
            KeyValuePair<TimeSpan, TimeSpan> result;
            if (Regex.IsMatch(TimeValue, ".+23:59.*")) {
                //メンテナンス終了後
                result = new KeyValuePair<TimeSpan, TimeSpan>(new TimeSpan(ParsePso2WebData.DefaultMaintenanceEndTime, 0, 0), new TimeSpan(23, 59, 59));
            } else if (Regex.IsMatch(TimeValue, ".*終日.*")) {
                //終日
                result = new KeyValuePair<TimeSpan, TimeSpan>(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));
            } else if (Regex.IsMatch(TimeValue, "[0-2]*[0-9].:[0-5]*[0-9].")) {
                //時間指定
                TimeSpan key = TimeSpan.Parse(TimeValue);
                result = new KeyValuePair<TimeSpan, TimeSpan>(key, key.Add(ParsePso2WebData.DefaultEmergencyTime));
            } else {
                //unknown type
                result = new KeyValuePair<TimeSpan, TimeSpan>(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));

            }
            return result;
        }
        /// <summary>
        /// イベントの種類の取得
        /// </summary>
        /// <param name="EventTypeText"></param>
        /// <returns></returns>
		internal EmergencyEventType GetEventType(string EventTypeText) {
            EmergencyEventType result = EmergencyEventType.UnKnown;
            switch (EventTypeText) {
                case "緊急":
                    result = EmergencyEventType.Emergency;
                    break;
                case "ブースト":
                    result = EmergencyEventType.Boost;
                    break;
                case "ネットカフェ":
                    result = EmergencyEventType.NetCafe;
                    break;
                case "チャレンジ":
                    result = EmergencyEventType.Challenge;
                    break;
                default:
                    break;
            }
            return result;
        }
        /// <summary>
        /// 対象日の取得
        /// </summary>
        /// <param name="DayNode"></param>
        /// <returns></returns>
		internal DateTime? GetDay(HtmlNode DayNode) {
            DateTime day = default(DateTime);
            if (ParsePso2WebData.DayRegexPattern.IsMatch(DayNode.InnerText)) {
                if (DateTime.TryParseExact(ParsePso2WebData.DayRegexPattern.Match(DayNode.InnerText).Value, "M月d日", null, DateTimeStyles.None, out day)) {
                    //年越し
                    if (DateTime.Now.Month < day.Month) {
                        day = day.AddYears(1);
                    }
                }
            }
            return (day == default(DateTime)) ? null : new DateTime?(day);
        }
	}
}
