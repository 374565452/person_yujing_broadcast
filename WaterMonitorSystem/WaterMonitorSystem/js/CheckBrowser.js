var browser = (function () {
    var useragent = navigator.userAgent,
    ua = useragent.toLowerCase(),
    browserlist = {
        msie: /(?:msie\s|trident.*rv:)([\w.]+)/i,
        firefox: /firefox\/([\w.]+)/i,
        chrome: /chrome\/([\w.]+)/i,
        safari: /version\/([\w.]+).*safari/i,
        opera: /(?:opr\/|opera.+version\/)([\w.]+)/i
    },
    kernels = {
        msie: /(compatible;\smsie\s|trident\/)[\w.]+/i,
        camino: /camino/i,
        khtml: /khtml/i,
        presto: /presto\/[\w.]+/i,
        gecko: /gecko\/[\w.]+/i,
        webkit: /applewebkit\/[\w.]+/i
    },
    browser = {
        kernel: 'unknow',
        version: 'unknow'
    }
    // 检测浏览器
    for (var i in browserlist) {
        var matchs = ua.match(browserlist[i]);
        browser[i] = matchs ? true : false;
        if (matchs) {
            browser.version = matchs[1];
        }
    }
    // 检测引擎
    for (var i in kernels) {
        var matchs = ua.match(kernels[i]);
        if (matchs) {
            browser.kernel = matchs[0];
        }
    }
    // 系统
    var os = ua.match(/(windows\snt\s|mac\sos\sx\s|android\s|ipad.*\sos\s|iphone\sos\s)([\d._-]+)/i);
    browser.os = os !== null ? os[0] : false;
    // 是否移动端
    browser.mobile = ua.match(/mobile/i) !== null ? true : false;
    return browser;
}());