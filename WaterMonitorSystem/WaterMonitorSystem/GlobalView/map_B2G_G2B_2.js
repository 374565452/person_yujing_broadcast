//val 要转换的坐标列表 ，每个坐标之间英文分号隔开，坐标形式“lng,lat”
function GABT(val, key) {
    this._val = val || "";
    this._key = key || "";
    this._num = 0;
    this._arr = {};
    this._curr = 0;
    this._digits = 8; //返回坐标小数点后位数
    this._returnStr = "";
    this._callback = null;

    //以下百度转谷哥可用参数
    this._amaplat = 0.00000000;
    this._amaplng = 0.00000000;
    this._delta_lat = 0;
    this._delta_lng = 0;

    this._n = 0;
}

GABT.prototype.ltrim = function (str) {
    if (str.charAt(0) == " ") 
    { 
        //如果字串左边第一个字符为空格 
        str = str.slice(1);//将空格从字串中去掉 
        //这一句也可改成 str = str.substring(1, str.length); 
        str = this.ltrim(str);   //递归调用 
    } 
    return str;
};

GABT.prototype.rtrim = function (str) {
    var ilength; 

    ilength = str.length;
    if (str.charAt(ilength - 1) == " ") 
    { 
        //如果字串右边第一个字符为空格 
        str = str.slice(0, ilength - 1);//将空格从字串中去掉 
        //这一句也可改成 str = str.substring(0, ilength - 1); 
        str = this.rtrim(str);   //递归调用 
    } 
    return str; 
};

//百度转谷歌，callback为回调函数，返回字符串
GABT.prototype.b2g = function (callback) {
    var ss = this.rtrim(this.ltrim(this._val));
    if (ss.length == 0) {
        callback(this._key, "error!");
        return;
    }

    this._callback = callback;

    this._arr = ss.split(';');

    this._curr = 0;
    this._num = this._arr.length;
    this._returnStr = "";

    this.b2gFitLocation(this._arr, 1, callback);
};

//百度地图转换为google地图，重复调用b2gFitLocation()，直至转过来的坐标经过转换后与百度原坐标差距小于0.00000002
GABT.prototype.b2gFitLocation = function (arr, k, callback) {
    if (this._curr >= this._num) {
        //callback("转换完成");
        callback(this._key, this._returnStr.substr(0, this._returnStr.length - 1));
        return;
    }

    if (k == 1) this._n = 1; else this._n++;
    if (k == 1) this._amaplng = 0.00000000;
    if (k == 1) this._amaplat = 0.00000000;

    if (!isNaN(this._amaplng) && !isNaN(this._amaplat)) {
        var point = new BMap.Point(this._amaplng, this._amaplat);
        var that = this;

        BMap.Convertor.translate(point, 2, function (p) {
            var result_lat = that._arr[that._curr].split(",")[1];
            var result_lng = that._arr[that._curr].split(",")[0];

            that._delta_lat = (p.lat - result_lat).toFixed(this._digits);
            that._delta_lng = (p.lng - result_lng).toFixed(this._digits);

            var abs_delta_lat = Math.abs(that._delta_lat * Math.pow(10, this._digits));
            var abs_delta_lng = Math.abs(that._delta_lng * Math.pow(10, this._digits));

            if (abs_delta_lat < 2 && abs_delta_lng < 2) {
                that._returnStr += that._amaplng.toFixed(this._digits) + "," + that._amaplat.toFixed(this._digits) + ";";
                if (that._curr < that._num) {
                    that._curr++;
                    that.b2gFitLocation(that._arr, 1, that._callback);
                }
            } else {
                that._amaplat = that._amaplat - that._delta_lat;
                that._amaplng = that._amaplng - that._delta_lng;
                that.b2gFitLocation(that._arr, 0, that._callback);
            }
        });
    }
    else {
        this._returnStr += "0,0;";
        this._curr++;
        this.b2gFitLocation(arr, 1, callback);
    }
};

//谷歌转百度，callback为回调函数，返回字符串
GABT.prototype.g2b = function (callback) {
    var ss = this.rtrim(this.ltrim(this._val));
    if (ss.length == 0) {
        callback(this._key, "error!");
        return;
    }

    this._callback = callback;

    this._arr = ss.split(';');
    this._curr = 0;
    this._num = this._arr.length;
    this._returnStr = "";

    this.g2bFitLocation(this._arr, callback);
};

//谷歌地图转百度地图
GABT.prototype.g2bFitLocation = function (arr, callback) {
    if (this._curr >= this._num) {
        //callback("转换完成");
        callback(this._key, this._returnStr.substr(0, this._returnStr.length - 1));
        return;
    }

    var amaplng = arr[this._curr].split(",")[0];
    var amaplat = arr[this._curr].split(",")[1];

    if (!isNaN(amaplng) && !isNaN(amaplat)) {
        var point = new BMap.Point(amaplng, amaplat);
        var that = this;
        BMap.Convertor.translate(point, 2, function (p) {
            that._returnStr += p.lng.toFixed(that._digits) + "," + p.lat.toFixed(that._digits) + ";";
            if (that._curr < that._num) {
                that._curr++;
                that.g2bFitLocation(that._arr, that._callback);
            }
        });
    }
    else {
        this._returnStr += "0,0;";
        this._curr++;
        this.g2bFitLocation(arr, callback);
    }
};
