using Giana.Api.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Giana.Api.Analysis.Activity;

public static class AuthorActivityCalculations
{
  public static IImmutableList<string> CreateActivityChartAsCsv(IImmutableList<GitLogRecord> records)
  {
    var authorFileTouches = CreateAuthorActivity(records);

    var sortedRecords = records.ToImmutableList().Sort((x, y) => x.Date.CompareTo(y.Date));
    var firstCommitDate = sortedRecords.First().Date;
    var lastCommitDate = sortedRecords.Last().Date;

    var listOfWeeks = CreateListOfWeeks(firstCommitDate, lastCommitDate);

    var authors = sortedRecords.OrderBy(rec => rec.Author).Select(rec => rec.Author).Distinct();

    return CreateChartDataGrid(authors.ToImmutableList(), listOfWeeks, authorFileTouches);
  }

  private static IImmutableList<AuthorActivity> CreateAuthorActivity(IImmutableList<GitLogRecord> records)
  {
    var commitsWithWeeks = records.Select(item => new CommitOfAuthor(
      Author: item.Author,
      Year: item.Date.Year,
      WeekOfYear: GetIso8601WeekOfYear(item.Date)));

    var authorFileTouches = new List<AuthorActivity>();
    var commitsGroupedByWeek = commitsWithWeeks.GroupBy(rec => rec.YearAndWeek);
    foreach (var weeks in commitsGroupedByWeek)
    {
      var commitsPerWeekAndAuthor = weeks.GroupBy(rec => rec.Author).ToArray();
      foreach (var group in commitsPerWeekAndAuthor)
      {
        authorFileTouches.Add(new AuthorActivity(
          Author: group.Key,
          YearAndWeek: group.First().YearAndWeek,
          TouchedFiles: group.Count()));
      }
    }

    return authorFileTouches.ToImmutableList();
  }

  private static IImmutableList<string> CreateListOfWeeks(DateTime begin, DateTime end)
  {
    var listOfWeeks = new List<string>();

    for (DateTime dt = begin; dt < end; dt += new TimeSpan(7, 0, 0))
    {
      int year = dt.Year;
      int week = GetIso8601WeekOfYear(dt);
      listOfWeeks.Add($"{year}-{week:00}");
    }

    return listOfWeeks.Distinct().ToImmutableList();
  }

  private static IImmutableList<string> CreateChartDataGrid(IImmutableList<string> authors, IImmutableList<string> yearAndWeeks, IImmutableList<AuthorActivity> fileTouches)
  {
    var chart = new List<string>();
    // Add column headers
    chart.Add("," + string.Join(",", yearAndWeeks));

    for (int author = 0; author < authors.Count; author++)
    {
      IImmutableList<string> row = ImmutableList.Create<string>();
      for (int yearAndWeek = 0; yearAndWeek < yearAndWeeks.Count; yearAndWeek++)
      {
        var commits = fileTouches.FirstOrDefault(rec => rec.Author == authors[author] && rec.YearAndWeek == yearAndWeeks[yearAndWeek]);
        if (commits != null)
        {
          row = row.Add(commits.TouchedFiles.ToString());
        }
        else
        {
          row = row.Add("0");
        }
      }
      chart.Add($"{authors[author]}," + string.Join(",", row));
    }

    return chart.ToImmutableList();
  }

  private static int GetIso8601WeekOfYear(DateTime time)
  {
    // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
    // be the same week# as whatever Thursday, Friday or Saturday are,
    // and we always get those right
    DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
    if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
    {
      time = time.AddDays(3);
    }

    // Return the week of our adjusted day
    return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
  }

  private sealed record CommitOfAuthor(string Author, int Year, int WeekOfYear)
  {
    public string YearAndWeek => $"{Year}-{WeekOfYear:00}";
  }
}
