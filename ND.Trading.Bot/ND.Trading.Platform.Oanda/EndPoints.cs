using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ND.Trading.Platform.Oanda
{
    public static class EndPoints
    {
        #region Account Endpoints
        public static readonly string LIST_ACCOUNTS = "/v3/accounts";
        public static readonly string ACCOUNT_INFO = "/v3/accounts/{accountId}";
        public static readonly string ACCOUNT_INSTRUMENTS = "/v3/accounts/{accountId}/instruments";
        public static readonly string ACCOUNT_SUMMARY = "/v3/accounts/{accountId}/summary";
        public static readonly string ACCOUNT_CHANGES = "/v3/accounts/{accountId}/changes";
        #endregion
        #region Positions Endpoints
        public static readonly string OPEN_POSITIONS = "/v3/accounts/{accountId}/openPositions";
        public static readonly string OPEN_INSTRUMENT_POSITIONS = "/v3/accounts/{accountId}/positions/";
        #endregion
        #region Trade Endpoints
        public static readonly string LIST_TRADES = "/v3/accounts/{accountId}/trades";
        public static readonly string LIST_ALL_OPEN_TRADES = "/v3/accounts/{accountId}/openTrades";
        public static readonly string LIST_OPEN_TRADES = "/v3/accounts/{accountId}/trades";
        public static readonly string CLOSE_TRADE = "/v3/accounts/{accountId}/trades/{tradeId}/close";
        #endregion
        #region Order Endpoints
        public static readonly string GET_ORDER = "/v3/accounts/{accountId}/orders/{orderId}";
        public static readonly string POST_ORDER = "/v3/accounts/{accountId}/orders";
        #endregion
        #region Transaction Endpoints
        public static readonly string LIST_TRANSACTIONS = "v3/accounts/{accountId}/transactions";
        #endregion

        #region Market Data Endpoints
        public static readonly string GET_CANDLES = "/v3/instruments/{instrument}/candles"; //? count = 6 & price = M & granularity = S5
        #endregion

        //https://api-fxtrade.oanda.com/v3/instruments/USD_CAD/candles?price=BA&from=2016-10-17T15%3A00%3A00.000000000Z&granularity=M1

        // public static readonly string PRICE = "/instruments"; //"https://api-fxtrade.oanda.com/v1/instruments?accountId=12345&instruments=AUD_CAD%2CAUD_CHF"
        ////"https://api-fxtrade.oanda.com/v1/accounts
        //public static readonly string GET_ACCOUNT = "/accounts"; //"https://api-fxtrade.oanda.com/v1/accounts/8954947"
        ////"https://api-fxtrade.oanda.com/v1/instruments?accountId=12345&instruments=AUD_CAD%2CAUD_CHF"
        //https://api-fxpractice.oanda.com/v1/""/instruments?accountId=621396
        //get order
        //"https://api-fxtrade.oanda.com/v1/accounts/12345/8d3c0b5d103d77462bd85dc55d2a6e1a-477dca1ec1d288c9ad17c7b2b7a04830/orders?instrument=EUR_USD&count=2"
        //post order
        //"instrument=EUR_USD&units=2&side=sell&type=market" "https://api-fxtrade.oanda.com/v1/accounts/12345/8d3c0b5d103d77462bd85dc55d2a6e1a-477dca1ec1d288c9ad17c7b2b7a04830/orders"
        // Get all positions - /v1/accounts/:account_id/positions 
        //Get position for an instrumet /v1/accounts/:account_id/positions/:instrument
    }
}

/*
 *  {
            "Name": "Oanda",
            "Market": "Forex",
            "BaseUrl": "https://api-fxpractice.oanda.com/v3/",
            "Key": "",
            "Secret": "",
            "Passphrase": "",
            "Token": ,
            "TickerList": "AUD/USD",
            "Currencies": "USD",
            "Strategies": [
                {
                    "Name": "Extrapolate",
                    "SellProfitPercent": "0.5",
                    "SellMode": "VARIABLE",
                    "FixedProfitValue": "5",
                    "PriceDownPercentage": "0.5",
                    "MinPositionLevel": "60",
                    "MaxPositionCount": "10",
                    "ExtrapolateCount": "15",
                    "ExtrapolateIndex": "4",
                    "PriceRangeStart": "0.001",
                    "PriceRangeEnd": "1000"
                }
            ]
        },

    */
