//val 要转换的坐标列表 ，每个坐标之间英文分号隔开，坐标形式“lng,lat”
function GABT4(arr) {
    this._arr = arr || [];
    this._num = 0;
    this._curr = 0;
    this._digits = 8; //返回坐标小数点后位数
    this._returnVal = [];
    this._callback = null;

    //以下百度转谷哥可用参数
    this._amaplat = 0.00000000;
    this._amaplng = 0.00000000;
    this._delta_lat = 0;
    this._delta_lng = 0;

    this._n = 0;
}

//百度转谷歌，callback为回调函数，返回字符串
GABT4.prototype.b2g = function (callback) {
    if (this._arr.length == 0) {
        callback(this._returnVal);
        return;
    }

    this._callback = callback;

    this._curr = 0;
    this._num = this._arr.length;
    this._returnVal = [];

    this.b2gFitLocation(this._arr, 1, callback);
};

//百度地图转换为google地图，重复调用b2gFitLocation()，直至转过来的坐标经过转换后与百度原坐标差距小于0.00000002
GABT4.prototype.b2gFitLocation = function (arr, k, callback) {
    if (this._curr >= this._num) {
        callback(this._returnVal);
        return;
    }

    if (k == 1) this._n = 1; else this._n++;
    if (k == 1) this._amaplng = 0.00000000;
    if (k == 1) this._amaplat = 0.00000000;
    if (!isNaN(this._amaplng) && !isNaN(this._amaplat)) {
        var pointGPS_BD = GPS.bd_encrypt(this._amaplat, this._amaplng);

        var result_lat = parseFloat(arr[this._curr].split(",")[1]);
        var result_lng = parseFloat(arr[this._curr].split(",")[0]);

        this._delta_lat = (pointGPS_BD.lat - result_lat).toFixed(this._digits);
        this._delta_lng = (pointGPS_BD.lon - result_lng).toFixed(this._digits);

        var abs_delta_lat = 0;
        try {
            abs_delta_lat = Math.abs(this._delta_lat * Math.pow(10, this._digits));
        } catch (e) {
            alert("lat：" + this._n + "," + this._curr + "," + result_lat + "," + result_lng + "," + this._delta_lat + "," + this._delta_lng);
        }

        var abs_delta_lng = 0;
        try {
            abs_delta_lng = Math.abs(this._delta_lng * Math.pow(10, this._digits));
        } catch (e) {
            alert("lng：" + this._n + "," + this._curr + "," + result_lat + "," + result_lng + "," + this._delta_lat + "," + this._delta_lng);
        }

        if (abs_delta_lat < 2 && abs_delta_lng < 2) {
            this._returnVal[this._curr] = this._amaplng.toFixed(this._digits) + "," + this._amaplat.toFixed(this._digits);
            if (this._curr < this._num) {
                this._curr++;
                this.b2gFitLocation(arr, 1, this._callback);
            }
        } else {
            this._amaplat = this._amaplat - this._delta_lat;
            this._amaplng = this._amaplng - this._delta_lng;
            this.b2gFitLocation(arr, 0, this._callback);
        }
    }
    else {
        this._returnVal[this._curr] = "0,0";
        this._curr++;
        this.b2gFitLocation(arr, 1, callback);
    }
};

//谷歌转百度，callback为回调函数，返回字符串
GABT4.prototype.g2b = function (callback) {
    if (this._val.length == 0) {
        callback(this._returnVal);
        return;
    }

    this._callback = callback;

    this._curr = 0;
    this._num = this._val.length;
    this._returnVal = [];

    this.g2bFitLocation(this._val, callback);
};

//谷歌地图转百度地图
GABT4.prototype.g2bFitLocation = function (arr, callback) {
    if (this._curr >= this._num) {
        //callback("转换完成");
        callback(this._returnVal);
        return;
    }

    var amaplat = arr[this._curr].split(",")[1];
    var amaplng = arr[this._curr].split(",")[0];

    if (!isNaN(amaplng) && !isNaN(amaplat)) {
        var pointGPS_GG = GPS.bd_decrypt(parseFloat(amaplat), parseFloat(amaplng));

        this._returnVal[this._curr] = pointGPS_GG.lng.toFixed(this._digits) + "," + pointGPS_GG.lat.toFixed(this._digits);
        if (this._curr < this._num) {
            this._curr++;
            this.g2bFitLocation(this._arr, this._callback);
        }
    }
    else {
        this._returnVal[this._curr] = "0,0";
        this._curr++;
        this.g2bFitLocation(this._arr, callback);
    }
};
