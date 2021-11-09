import time
import os
import tinvest
import datetime

token = ("t.F-e_MvGHyM5RydIcD28rwvIuOpgfChfOokIlqKWYOm9JKUeJFLQwlZMP0O6p_hneiDWOAjT90UQzJSlEvBZSog")
client = tinvest.SyncClient(token)

figi = client.get_market_stocks()
for i in figi:
    print(i)
