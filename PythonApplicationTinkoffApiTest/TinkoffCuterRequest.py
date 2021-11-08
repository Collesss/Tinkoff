import datetime
import tinvest

class TinkoffCuterRequest(object):
	fromTime : datetime.datetime
	toTime : datetime.datetime
	candleTime : tinvest.CandleResolution

	def __init__(self, fromTime : datetime.datetime, toTime : datetime.datetime, candleTime : tinvest.CandleResolution):
		self.fromTime = fromTime
		self.toTime = toTime
		self.candleTime = candleTime
		pass


