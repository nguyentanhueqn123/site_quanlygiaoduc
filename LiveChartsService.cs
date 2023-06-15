using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.ViewModels;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using GymManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace GymManagement.Services
{
    class LiveChartsService 
    {
        private static GymManagementDbContext _dbcontext;
        #region Methods

       
        private static int TotalContractsCountByDay(DateTime day)
        {
            using (_dbcontext = new GymManagementDbContext())
            {
                return _dbcontext.Contracts.Where(s => s.CreateDate.Value == day).Count();
            }
        }


        private static int TotalContractsCountByMonth(DateTime month)
        {
            using (_dbcontext = new GymManagementDbContext())
            {
                return _dbcontext.Contracts.Where(s => s.CreateDate.Value.Month == month.Month && s.CreateDate.Value.Year == month.Year).Count();
            }
        }

        private static int TotalBookingsCountToDay(DateTime day)
        {
            using (_dbcontext = new GymManagementDbContext())
            {
                return _dbcontext.Bookings.Where(s => s.CreateDate.Value == day).Count();
            }
        }
        private static int TotalRevenueCountByMonth(DateTime month)
        {
            int money = 0;
            using(_dbcontext = new GymManagementDbContext())
            {
                foreach(Contract contract in _dbcontext.Contracts.Include(s => s.Course).Where(s => s.CreateDate.Value.Month == month.Month && s.CreateDate.Value.Year == month.Year).ToList())
                {
                    money += (int) contract.Course.Price;
                }
            }
            return money;
        }
        
        #endregion

        #region Title

        public static LabelVisual TitleTotalRevenue { get; set; } =
            new LabelVisual
            {
                Text = "Doanh thu của phòng Gym",

                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
        public static LabelVisual TitleContract { get; set; } =
           new LabelVisual
           {
               Text = "Tổng số hợp đồng đã được tạo trong 7 ngày gần nhất",
               TextSize = 25,
               Padding = new LiveChartsCore.Drawing.Padding(15),
               Paint = new SolidColorPaint(SKColors.DarkSlateGray)
           };

        public static LabelVisual TitleBookingInAWeek { get; set; } =
           new LabelVisual
           {
               Text = "Số lần đặt lịch trong 7 ngày vừa qua",

               TextSize = 25,
               Padding = new LiveChartsCore.Drawing.Padding(15),
               Paint = new SolidColorPaint(SKColors.DarkSlateGray)
           };
        #endregion

        #region Value<DateTimePoint>
        public static IEnumerable<DateTimePoint> TotalRevenueInAYear()
        {
            int numberofmonths = 12;
            var Value = new ObservableCollection<DateTimePoint>();
            for (int i = 0; i < numberofmonths; i++)
            {
                Value.Add(new DateTimePoint(DateTime.Today.AddMonths(-i), TotalRevenueCountByMonth(DateTime.Today.AddMonths(-i))));
            }
            return Value;
        }
        public static IEnumerable<DateTimePoint> TotalContractInAWeek()
        {
            int numberofday = 7;
            var Value = new ObservableCollection<DateTimePoint>();
            for (int i = 0; i < numberofday; i++)
            {
                Value.Add(new DateTimePoint(DateTime.Today.AddDays(-i), TotalContractsCountByDay(DateTime.Today.AddDays(-i))));
            }
            return Value;
        }
        public static IEnumerable<DateTimePoint> TotalContractInAMonth()
        {
            int numberofday = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var Value = new ObservableCollection<DateTimePoint>();
            for (int i = 0; i < numberofday; i++)
            {
                Value.Add(new DateTimePoint(DateTime.Today.AddDays(-i), TotalContractsCountByDay(DateTime.Today.AddDays(-i))));
            }
            return Value;
        }
        public static IEnumerable<DateTimePoint> TotalContractInAYear()
        {
            int numberofmonths = 12;
            var Value = new ObservableCollection<DateTimePoint>();
            for (int i = 0; i < numberofmonths; i++)
            {
                Value.Add(new DateTimePoint(DateTime.Today.AddMonths(-i), TotalContractsCountByMonth(DateTime.Today.AddMonths(-i))));
            }
            return Value;
        }
        public static IEnumerable<DateTimePoint> TotalBookingsToDay()
        {
            int numberofday = 7;
            var Value = new ObservableCollection<DateTimePoint>();
            for (int i = 0; i < numberofday; i++)
            {
                Value.Add(new DateTimePoint
                    (DateTime.Today.AddDays(-i), TotalBookingsCountToDay(DateTime.Today.AddDays(-i))));
            }
            return Value;
        }
        #endregion


        #region ISeries
        public static ISeries[] TotalRevenueOneYearSeries()
        {
            return new ISeries[]
            {
                new LineSeries<DateTimePoint>
                            {

                                TooltipLabelFormatter = (chartPoint) =>
                    $"{new DateTime((long) chartPoint.SecondaryValue).ToString("MMMM",CultureInfo.GetCultureInfo("vi"))}: { ( chartPoint.PrimaryValue).ToString("#,##0" + " VND")}",

                                Values = TotalRevenueInAYear()
                    }


            };
        }
        public static ISeries[] TotalContractInAWeekSeries()
        {
            return new ISeries[]
            {
                 new ColumnSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) =>
                    $"{new DateTime((long) chartPoint.SecondaryValue).ToString("dddd",CultureInfo.GetCultureInfo("vi"))}: {chartPoint.PrimaryValue.ToString("#" +" hợp đồng")}",
                Values = TotalContractInAWeek()
            }
            };
        }



        public static ISeries[] TotalContractInAMonthOneMonthSeries()
        {
            return new ISeries[]
            {
                 new ColumnSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) =>
                    $"{new DateTime((long) chartPoint.SecondaryValue):MMM dd}: {chartPoint.PrimaryValue}",
                Values = TotalContractInAMonth()
            }
            };
        }

        public static ISeries[] TotalContractInAYearSeries()
        {
            return new ISeries[]
            {
                new ColumnSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) =>
                    $"{new DateTime((long) chartPoint.SecondaryValue).ToString("MMMM",CultureInfo.GetCultureInfo("vi"))}: {chartPoint.PrimaryValue}",

                Values = TotalContractInAYear()
            }
            };
        }
        public static ISeries[] TotalBookingsToDaySeries()
        {
            return new ISeries[]
            {
                 new LineSeries<DateTimePoint>
            {
                TooltipLabelFormatter = (chartPoint) =>
                    $"{new DateTime((long) chartPoint.SecondaryValue).ToString("dddd",CultureInfo.GetCultureInfo("vi"))}: {chartPoint.PrimaryValue.ToString("#" +" lần")}", 
                Values = TotalBookingsToDay()
            }
            };
        }

        
        #endregion

        #region Axis

        
        public static Axis[] OneWeekXAxes { get; set; } =
        {
            
            new Axis
            {
                
                Labeler = value => new DateTime((long) value).ToString("dddd",CultureInfo.GetCultureInfo("vi")),
                LabelsRotation = 10,
                Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 0),
                
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                
                MinStep = TimeSpan.FromDays(1).Ticks
            }
        };
        public static Axis[] OneMonthXAxes { get; set; } =
        {
            new Axis
            {


                Labeler = value => new DateTime((long) value).ToString("dd",CultureInfo.GetCultureInfo("vi")),
                LabelsRotation = 10,

                Padding = new LiveChartsCore.Drawing.Padding(0,0,0,0),
                UnitWidth = TimeSpan.FromDays(1).Ticks,


                MinStep = TimeSpan.FromDays(1).Ticks
            }
        };

        public static Axis[] OneYearXAxes { get; set; } =
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("MMMM",CultureInfo.GetCultureInfo("vi")),
                LabelsRotation = 50,
                Padding = new LiveChartsCore.Drawing.Padding(0,0,0,0),



                UnitWidth = TimeSpan.FromDays(30.4375).Ticks,

                MinStep = TimeSpan.FromDays(28).Ticks
            }
        };
        #endregion
    }
}
