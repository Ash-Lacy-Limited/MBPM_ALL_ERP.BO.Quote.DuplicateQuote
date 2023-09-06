// When Duplicating Quotes copy the originating Quote line Qty's
System.IO.StreamWriter STDOUT = (System.IO.StreamWriter)null;

const string Method = "BPM-Quote-DuplicateQuote-Post";
string OutPutFile = string.Format(@"C:\Temp\{0}.txt", Method);

bool Debug = false;

try
{
    if (Debug)
        STDOUT = new System.IO.StreamWriter
        (new System.IO.FileStream(OutPutFile, System.IO.FileMode.Append));
    else
        STDOUT = new System.IO.StreamWriter(System.IO.Stream.Null);

    STDOUT.WriteLine(string.Format("{0} - Start.", Method));


    foreach (var ttQuoteHedRow in result.QuoteHed.Where(x => x.Company == Session.CompanyID))
    {
        var NewQuoteNum = ttQuoteHedRow.QuoteNum;
        STDOUT.WriteLine(string.Format("This QuoteNum:{0}, Original Quote: {1}, Original Lines: {2}", NewQuoteNum, sourceQuote, sourceLines));
        bool bAllLines = string.IsNullOrEmpty(sourceLines);
        sourceLines = "~" + sourceLines + "~";
        int newQuoteLine = 1;
        int counter = 0;
       
        using (var txScope = IceContext.CreateDefaultTransactionScope())
        {
            foreach (var OrigQuoteDtl in Db.QuoteDtl.Where(x => x.Company == Session.CompanyID && x.QuoteNum == sourceQuote))
            {
                string quoteLine = "~" + OrigQuoteDtl.QuoteLine + "~";
                STDOUT.WriteLine(string.Format("Original QuoteNum: {0}, QuoteLine: {1}", OrigQuoteDtl.QuoteNum, quoteLine));

                if (bAllLines || sourceLines.Contains(quoteLine))
                {
                    STDOUT.WriteLine(string.Format("{0} | {1} | {2} | {3}", OrigQuoteDtl.QuoteNum, OrigQuoteDtl.OrderQty, OrigQuoteDtl.SellingExpectedQty, counter));
                    var NewQuoteDtl = Db.QuoteDtl.Where(x => x.Company == Session.CompanyID && x.QuoteNum == NewQuoteNum && x.QuoteLine == newQuoteLine).FirstOrDefault();             
                    if (NewQuoteDtl != null)
                    {
                            QuoteSvcContract quoteBO = Ice.Assemblies.ServiceRenderer.GetService<QuoteSvcContract>(Db);
                            QuoteTableset quoteTS = quoteBO.GetByID(NewQuoteDtl.QuoteNum);
                            quoteTS.QuoteDtl[counter].OrderQty = OrigQuoteDtl.OrderQty;
                            quoteTS.QuoteDtl[counter].SellingExpectedQty = OrigQuoteDtl.SellingExpectedQty;
                            quoteTS.QuoteDtl[counter].RowMod = "U";
                            quoteBO.Update(ref quoteTS);
                            newQuoteLine++;
                            counter++;
                    }
                }
            }
            txScope.Complete();
        }
    }
}
catch (Exception ex)

{
    STDOUT.WriteLine(string.Format("Error: {0} - {1}", Method, ex.Message));
}

finally
{
    STDOUT.WriteLine(string.Format("{0} ----END---", Method));
    STDOUT.Close();
}
