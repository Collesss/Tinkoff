import time
import os
import tinvest

import datetime
import TinkoffCuterRequest as tcr

tinkoff : tcr.TinkoffCuterRequest = tcr.TinkoffCuterRequest(datetime.datetime.now() - datetime.timedelta(100), datetime.datetime.now(), tinvest.CandleResolution.hour)



print(tinkoff.fromTime);
print(tinkoff.toTime);
print(tinkoff.candleTime);

token = ("t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog")
client = tinvest.SyncClient(token)

print(dir(client))

figi = client.get_market_stocks()
for i in figi:
    print (i)
