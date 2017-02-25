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
        var pointGPS_BD = GPS.bd_encrypt(this._amaplat, this._amaplng);

        var result_lat = parseFloat(this._arr[this._curr].split(",")[1]);
        var result_lng = parseFloat(this._arr[this._curr].split(",")[0]);

        this._delta_lat = (pointGPS_BD.lat - result_lat).toFixed(this._digits);
        this._delta_lng = (pointGPS_BD.lon - result_lng).toFixed(this._digits);

        var abs_delta_lat = Math.abs(this._delta_lat * Math.pow(10, this._digits));
        var abs_delta_lng = Math.abs(this._delta_lng * Math.pow(10, this._digits));

        if (abs_delta_lat < 2 && abs_delta_lng < 2) {
            this._returnStr += this._amaplng.toFixed(this._digits) + "," + this._amaplat.toFixed(this._digits) + ";";
            if (this._curr < this._num) {
                this._curr++;
                this.b2gFitLocation(this._arr, 1, this._callback);
            }
        } else {
            this._amaplat = this._amaplat - this._delta_lat;
            this._amaplng = this._amaplng - this._delta_lng;
            this.b2gFitLocation(this._arr, 0, this._callback);
        }
    }
    else {
        this._returnStr += "0,0;";
        this._curr++;
        this.b2gFitLocation(this._arr, 1, callback);
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
        var pointGPS_GG = GPS.bd_decrypt(parseFloat(amaplat), parseFloat(amaplng));

        this._returnStr += pointGPS_GG.lng.toFixed(this._digits) + "," + pointGPS_GG.lat.toFixed(this._digits) + ";";
        if (this._curr < this._num) {
            this._curr++;
            this.g2bFitLocation(this._arr, that._callback);
        }
    }
    else {
        this._returnStr += "0,0;";
        this._curr++;
        this.g2bFitLocation(this._arr, callback);
    }
};
