Name: ERP.BO.Quote.DuplicateQuote
Author: Richard Baker
Date: 06/09/2023

Confluence Link: https://ashandlacy.atlassian.net/wiki/spaces/KU/pages/59867137/Erp.BO.Quote.DuplicateQuote

******************
*****Comments*****
******************
All method directives for duplicating a quote
1) Copy quantities over from the old to the new quote
2) Copy the UD108 and UD108A data (optimiser data) from the old quote to the new quote
3) Reset the optimiser flag for the new quote
4) Removes any discounts on the new quote at both the header and detail level
