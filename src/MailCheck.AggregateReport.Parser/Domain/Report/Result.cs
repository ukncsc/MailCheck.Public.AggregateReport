﻿//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MailCheck.AggregateReport.Parser.Domain.Report
//{
//    public class Result<TDomain> where TDomain : class
//    {
//        public static Result<TDomain> FailedResult = new Result<TDomain>(null, false, false);

//        public Result(TDomain report, bool success, bool duplicate)
//        {
//            Report = report;
//            Success = success;
//            Duplicate = duplicate;
//        }

//        public TDomain Report { get; }
//        public bool Success { get; }
//        public bool Duplicate { get; }
//    }
//}
