/* 
作者：  于新海
最后整理时间：  2013.5.1

jv 有四种对象 
一是 jv.Root 返回的是   /udi/
二是 jv.AllRequest 记录了本页面加载的Page记录。
三是 jv.AddRequest 添加页面Request。
四是 jv.UnloadRequest页面退出时，清除最后一个 Request
 
JQuery1.4.4 的修改记录： 使用 jv.IsNull 和 jv.CreateEventSource(xhr); 两处.

jQuery 的 Ajax 可以追溯到 Ajax 之前是的事件源了。

属性命名： 采用 驼峰命名法
方法命名：采用 驼峰命名法 和 匈牙利命名法 匀可。
*/

//设置多次执行阻止。

//Jv JavaScript Var.
if (!window.jv) {
    jv = window.jv = {};
}

if (!jv.AllRequest) {
    jv.AllRequest = Array();
}

(function () {

    //本页标志性含数 getDoer .
    if (jv.getDoer) return;

    jv.InModal = jv.inModal = function () {
        if (typeof (window.dialogArguments) != "undefined" && typeof (window.opener) == "undefined") {
            return true;
        }
        return false;
    }


    if (jv.InModal()) { opener = window.dialogArguments; }

    jv.MyLoadOn = (jv.MyLoadOn || {});
    jv.MyInitOn = (jv.MyInitOn || {});

    //阻止事件冒泡,静态写法 udi
    jv.CancelBubble = jv.cancelBubble = function (e) {
        if (e.stopPropagation) {
            e.stopPropagation();
        }
        // otherwise set the cancelBubble property of the original event to true (IE)
        e.cancelBubble = true;
    }


    jQuery.param = function (a, traditional) {

        var buildParams = function (prefix, obj, traditional, add) {
            if (jQuery.isArray(obj)) {
                // Serialize array item.
                jQuery.each(obj, function (i, v) {
                    if (traditional || /\[\]$/.test(prefix)) {
                        // Treat each array item as a scalar.
                        add(prefix, v);

                    } else {
                        // If array item is non-scalar (array or object), encode its
                        // numeric index to resolve deserialization ambiguity issues.
                        // Note that rack (as of 1.0.0) can't currently deserialize
                        // nested arrays properly, and attempting to do so may cause
                        // a server error. Possible fixes are to modify rack's
                        // deserialization algorithm or to provide an option or flag
                        // to force array serialization to be shallow.
                        buildParams(prefix + "[" + i + "]", v, traditional, add);
                    }
                });

            } else if (!traditional && obj != null && typeof obj === "object") {
                // Serialize object item.
                for (var name in obj) {
                    buildParams(prefix + "." + name, obj[name], traditional, add);
                }

            } else {
                // Serialize scalar item.
                add(prefix, obj);
            }
        };


        var s = [], add = function (key, value) {
            // If value is a function, invoke it and return its value
            value = jQuery.isFunction(value) ? value() : value;
            s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
        };

        // Set traditional to true for jQuery <= 1.3.2 behavior.
        if (traditional === undefined) {
            traditional = jQuery.ajaxSettings.traditional;
        }

        // If an array was passed in, assume that it is an array of form elements.
        if (jQuery.isArray(a) || (a.jquery && !jQuery.isPlainObject(a))) {
            // Serialize the form elements
            jQuery.each(a, function () {
                add(this.name, this.value);
            });

        } else {
            // If traditional, encode the "old" way (the way 1.3.2 or older
            // did it), otherwise encode params recursively.
            for (var prefix in a) {
                buildParams(prefix, a[prefix], traditional, add);
            }
        }

        // Return the resulting serialization
        return s.join("&").replace(/%20/g, "+");
    };


    jv.DefineProperty = jv.defineProperty = function (target, functionName, func, isEnumerable) {
        if (!Object.defineProperty) {
            target[functionName] = func;
        }
        else {
            try {
                Object.defineProperty(target, functionName, { value: func, enumerable: isEnumerable });
            }
            catch (e) {
                target[functionName] = func;
            }
        }
    }

    //对象的继承，采用 TypeScript 的继承方式。  jv.extend( child,base) ; 即可。
    jv.Extend = jv.extend = function (child, base) {
        function __() { this.constructor = child; }
        __.prototype = base.prototype;
        child.prototype = new __();
    };

    //Udi 备份一下原来的方法
    String.prototype.Split = String.prototype.split;
    {
        //http://blog.stevenlevithan.com/archives/cross-browser-split
        /*!
        * Cross-Browser Split 1.1.1
        * Copyright 2007-2012 Steven Levithan <stevenlevithan.com>
        * Available under the MIT License
        * ECMAScript compliant, uniform cross-browser split method
        */

        /**
        * Splits a string into an array of strings using a regex or string separator. Matches of the
        * separator are not included in the result array. However, if `separator` is a regex that contains
        * capturing groups, backreferences are spliced into the result each time `separator` is matched.
        * Fixes browser bugs compared to the native `String.prototype.split` and can be used reliably
        * cross-browser.
        * @param {String} str String to split.
        * @param {RegExp|String} separator Regex or string to use for separating the string.
        * @param {Number} [limit] Maximum number of items to include in the result array.
        * @returns {Array} Array of substrings.
        * @example
        *
        * // Basic use
        * split('a b c d', ' ');
        * // -> ['a', 'b', 'c', 'd']
        *
        * // With limit
        * split('a b c d', ' ', 2);
        * // -> ['a', 'b']
        *
        * // Backreferences in result array
        * split('..word1 word2..', /([a-z]+)(\d+)/i);
        * // -> ['..', 'word', '1', ' ', 'word', '2', '..']
        */
        var split;

        // Avoid running twice; that would break the `nativeSplit` reference
        split = split || function (undef) {

            var nativeSplit = String.prototype.split,
        compliantExecNpcg = /()??/.exec("")[1] === undef, // NPCG: nonparticipating capturing group
        self;

            self = function (str, separator, limit) {
                // If `separator` is not a regex, use `nativeSplit`
                if (Object.prototype.toString.call(separator) !== "[object RegExp]") {
                    return nativeSplit.call(str, separator, limit);
                }
                var output = [],
            flags = (separator.ignoreCase ? "i" : "") +
                    (separator.multiline ? "m" : "") +
                    (separator.extended ? "x" : "") + // Proposed for ES6
                    (separator.sticky ? "y" : ""), // Firefox 3+
            lastLastIndex = 0,
                // Make `global` and avoid `lastIndex` issues by working with a copy
            separator = new RegExp(separator.source, flags + "g"),
            separator2, match, lastIndex, lastLength;
                str += ""; // Type-convert
                if (!compliantExecNpcg) {
                    // Doesn't need flags gy, but they don't hurt
                    separator2 = new RegExp("^" + separator.source + "$(?!\\s)", flags);
                }
                /* Values for `limit`, per the spec:
                * If undefined: 4294967295 // Math.pow(2, 32) - 1
                * If 0, Infinity, or NaN: 0
                * If positive number: limit = Math.floor(limit); if (limit > 4294967295) limit -= 4294967296;
                * If negative number: 4294967296 - Math.floor(Math.abs(limit))
                * If other: Type-convert, then use the above rules
                */
                limit = limit === undef ?
            -1 >>> 0 : // Math.pow(2, 32) - 1
            limit >>> 0; // ToUint32(limit)
                while (match = separator.exec(str)) {
                    // `separator.lastIndex` is not reliable cross-browser
                    lastIndex = match.index + match[0].length;
                    if (lastIndex > lastLastIndex) {
                        output.push(str.slice(lastLastIndex, match.index));
                        // Fix browsers whose `exec` methods don't consistently return `undefined` for
                        // nonparticipating capturing groups
                        if (!compliantExecNpcg && match.length > 1) {
                            match[0].replace(separator2, function () {
                                for (var i = 1; i < arguments.length - 2; i++) {
                                    if (arguments[i] === undef) {
                                        match[i] = undef;
                                    }
                                }
                            });
                        }
                        if (match.length > 1 && match.index < str.length) {
                            Array.prototype.push.apply(output, match.slice(1));
                        }
                        lastLength = match[0].length;
                        lastLastIndex = lastIndex;
                        if (output.length >= limit) {
                            break;
                        }
                    }
                    if (separator.lastIndex === match.index) {
                        separator.lastIndex++; // Avoid an infinite loop
                    }
                }
                if (lastLastIndex === str.length) {
                    if (lastLength || !separator.test("")) {
                        output.push("");
                    }
                } else {
                    output.push(str.slice(lastLastIndex));
                }
                return output.length > limit ? output.slice(0, limit) : output;
            };

            // For convenience
            String.prototype.split = function (separator, limit) {
                return self(this, separator, limit);
            };

            return self;

        }();
    };


    jv.defineProperty(String.prototype, 'format', function () {
        var args = arguments;
        return this.replace(/\{(\d+)\}/g,
                //m 是指搜索到的整个部分， 如： {0} , 而 i  是指 该部分的分组内容 ， 如 0
                function (m, i) {
                    return args[i];
                });
    });

    //参数是 Json 的 format。
    jv.defineProperty(String.prototype, 'formatEx', function () {
        var jsv = arguments[0];
        return this.replace(/\{(\w+)\}/g,
            //m 是指搜索到的整个部分， 如： {id} , 而 i  是指 该部分的分组内容 ， 如 id
            function (m, i) {
                if (i in jsv) return jsv[i];
                else return m;
            });
    });


    jv.defineProperty(String.prototype, 'padLeft', function (Length, Char) {
        var addtional = "";
        for (var i = 0, len = Length - this.length; i < len; i++) {
            addtional += Char;
        }
        return addtional + this;
    });


    jv.defineProperty(String.prototype, 'padRight', function (Length, Char) {
        var addtional = "";
        for (var i = 0, len = Length - this.length; i < len; i++) {
            addtional += Char;
        }
        return this + addtional;
    });


    jv.defineProperty(String.prototype, 'trimAll', function () {
        return this.replace(/\s+/g, "");
    });


    jv.defineProperty(String.prototype, 'trim', function () {
        return this.replace(/(^\s*)|(\s*$)/g, "");
    });




    //按字符来计算字符串大小. 一个双字节 按2个计算.
    jv.defineProperty(String.prototype, 'byteLen', function () {
        return this.replace(/[^\u0000-\u00ff]/g, "--").length;
    });


    //取日期, 字符串可以用 - , . 分隔.
    jv.defineProperty(String.prototype, 'getDate', function () {
        if (!this.toString()) return Date.MinValue;

        var val = this.replace(/\-|\,|\.|年|月|日/g, "/");

        if ((val == "0001/01/01") || (val == "0001/01/01 00:00:00") || (val == "0001/01/01 00:00") || (val == "0001/1/1 00:00:00") || (val == "0001/1/1 00:00")) return Date.MinValue;
        return new Date(Date.parse(val));
    });


    jv.defineProperty(String.prototype, 'isGuid', function () {
        //6B29FC40-CA47-1067-B31D-00DD010662DA
        var val = this.toLowerCase().mySplit("-", true);
        if (val.length != 5) return false;

        //parseInt("10DD010662DA", 16).toString(16)
        var IsHex = function (str) {

            if (str.slice(0, 1) == "0") {
                str = "1" + str.slice(1);
            }
            return parseInt(str, 16).toString(16) == str;
        };

        if (val[0].length != 8 ||
                 val[1].length != 4 || val[2].length != 4 || val[3].length != 4 ||
                         val[4].length != 12) {
            return false;
        }

        return IsHex(val[0]) && IsHex(val[1]) && IsHex(val[2]) && IsHex(val[3]) && IsHex(val[4]);
    });


    //保持和C#同样的格式。yyyyMMdd HHmmss
    jv.defineProperty(Date.prototype, 'toString', function () {
        var arg = arguments[0], jsv = arg;
        if (!jsv) jsv = "yyyy-MM-dd";

        var y = this.getFullYear(), m = this.getMonth(), d = this.getDate(), h = this.getHours(), mi = this.getMinutes(), s = this.getSeconds();
        if (y == 1 || y == 1901) return "";

        if (!arg) {
            if (h > 0 || mi > 0 || s > 0) {
                jsv += " HH:mm";

                if (s > 0) {
                    jsv += ":ss";
                }
            }
        }


        return jsv.replace(new RegExp("yyyy", "g"), y)
        .replace(new RegExp("MM", "g"), (m + 1).toString().padLeft(2, "0"))
        .replace(new RegExp("dd", "g"), d.toString().padLeft(2, "0"))
        .replace(new RegExp("HH", "g"), h.toString().padLeft(2, "0"))
        .replace(new RegExp("mm", "g"), mi.toString().padLeft(2, "0"))
        .replace(new RegExp("ss", "g"), s.toString().padLeft(2, "0"))
        .replace(new RegExp("yy", "g"), y.toString().slice(2))
        .replace(new RegExp("M", "g"), m + 1)
        .replace(new RegExp("d", "g"), d)
        .replace(new RegExp("hh", "g"), h)
        .replace(new RegExp("m", "g"), mi)
        .replace(new RegExp("s", "g"), s);

    });

    jv.defineProperty(Date.prototype, 'addDays', function () {
        var days = arguments[0];
        if (!days) return this;

        var y = this.getFullYear(), m = this.getMonth(), d = this.getDate(), h = this.getHours(), mi = this.getMinutes(), s = this.getSeconds();

        return new Date(this.valueOf() + days * 24 * 60 * 60 * 1000);
    });

    //
    jv.defineProperty(Array.prototype, 'indexOf', function (item, i) {
        i || (i = 0);
        var length = this.length;
        if (i < 0) i = length + i;
        for (; i < length; i++) {
            if (this[i] === item) return i;
        }
        return -1;
    });

    //仅支持回调的indexOf
    jv.IndexOf = jv.indexOf = function (ary, func) {
        if (jv.IsNull(ary)) return -1;
        for (var i = 0, len = ary.length; i < len; i++) {
            if (func(ary[i]) === true) return i;
        }
        return -1;
    }



    jv.defineProperty(Array.prototype, 'max', function () {
        if (!$.isArray(this)) return false;
        return Math.max.apply({}, this)
    });



    jv.defineProperty(Array.prototype, 'min', function () {
        if (!$.isArray(this)) return false;
        return Math.min.apply({}, this)
    });


    jv.defineProperty(Array.prototype, 'removeAt', function (dx) {
        this.splice(dx, 1);
        return this;
    });

    jv.defineProperty(Array.prototype, 'insertAt', function (dx, obj) {
        this.splice(dx, 0, obj);
        return this;
    });


    //删除的对象支持回调
    jv.defineProperty(Array.prototype, 'remove', function (n) {
        var index = this.indexOf(n);
        if (index >= 0) {
            return this.removeAt(index);
        }
    });

    //需要对象支持 removeAt
    jv.Remove = jv.remove = function (ary, func) {
        ary.removeAt(jv.indexOf(ary, func));
    };



    /*
    * 求两个集合的交集, callback 返回 true 则为真.
    */
    jv.defineProperty(Array.prototype, 'intersect', function (b, callback) {
        callback = callback || function (a, b) { return a == b; };

        var ret = [];
        $(this).each(function (i, d) {
            $(b).each(function (_i, _d) {
                if (callback(d, _d)) ret.push(d);
            });
        });
        return ret;
    });


    //求差集, callback 是判断相等的回调.返回 true,则相减.
    jv.defineProperty(Array.prototype, 'minus', function (b, callback) {
        if (jv.IsNull(b)) return this;
        if (b.length == 0) return this;

        callback = callback || function (a, b) { return a == b; };
        var ret = [];
        $.each(this, function () {
            var one = this;
            $.each(b, function () {
                var other = this;
                var retVal = callback(one, other);
                if (!jv.IsNull(retVal) && retVal) {
                    ret.push(one);
                }
            });
        });
        return ret;
        // a.uniquelize().each(function (o) { return b.contains(o) ? null : o });

    });


    //Js 的 Lambda First , 慎用.  $([1,3,5]).toArray().lmFirst
    jv.defineProperty(Array.prototype, 'lmFirst', function (callback) {
        var jthis = $(this);
        return jthis.filter(function (i) {
            var one = this;
            if (one && jthis.hasOwnProperty(i)) {
                if ($.isFunction(callback))
                    return callback(i, one);
                else return false;
            }
            else return false;
        })[0];
    });




    //回调返回false  停止. 调用示例:
    /*
    var ary = [{"id":"2","cell":["0","0","报事处理","","2","Yes",""], "rows":
    [{"id":"3","cell":["2","0,2","报事查询","~/cs/treception/ReceptionQuery.aspx?History=True","3","Yes",""]},
    {"id":"4","cell":["2","0,2","接待登记","~/cs/Reception/ReceptionIndex.aspx?History=True","4","Yes",""]}
    ]
    ]

    ary.Recursion( function(container,i,d){
    return d.rows ;
    },function(container,i,d){
    if ( d.id == 3 ) return false ;
    }) ;
    */
    jv.defineProperty(Array.prototype, 'Recursion', function (FindSubs, ExecFunc) {
        for (var i = 0; i < this.length; i++) {
            var item = this[i];
            if (!item) continue;
            //传的三个参数表示： 容器，容器当前索引，当前索引项
            if (ExecFunc(this, i, item) == false) return false;

            //传的三个参数表示： 容器，容器当前索引，当前索引项
            var subs = FindSubs(this, i, item);
            if (subs && subs.Recursion && subs.Recursion(FindSubs, ExecFunc) == false) {
                return false;
            }
        }
        return true;
    });

    //separator 分隔符，支持字符串。
    jv.defineProperty(String.prototype, 'mySplit', function (separator, removeEmpty) {
        if (removeEmpty) {
            return $.grep(this.split(separator), function (d, i) {
                return !jv.IsNull(d) && d.length;
            });
        }
        else return this.split(separator);


    }, false);

    //separator 分隔符，支持字符串。
    jv.defineProperty(String.prototype, 'insert', function (index, str) {
        return this.substring(0, index) + str + this.substr(index);
    }, false);

    if ($.browser.msie) {
        jv.defineProperty(Number.prototype, 'toFixed', function (fractionDigits) {
            fractionDigits = fractionDigits || 0;
            if (fractionDigits < 0) fractionDigits = 0;

            var val = parseInt(this * Math.pow(10, fractionDigits) + 0.5).toString();
            if (fractionDigits === 0) return val;
            return val.insert(val.length - fractionDigits, ".");
        }, false);
    }

    var _Date_MinValue = new Date(0);
    _Date_MinValue.setFullYear(1);
    _Date_MinValue.setHours(0);

    Date.MinValue = _Date_MinValue;


    //reg 是 正则表达式对象.示例:尽量不要使用。
    //jv.SplitWithReg("{yyyy} 年 {MM} 月 {dd} 日", new RegExp( "\{\\w+\}","g") )
    jv.SplitWithReg = function (Source, reg) {
        //http://blog.stevenlevithan.com/archives/cross-browser-split

        var retVal = [],
     matchs = Source.match(reg),
     array = Source.split(reg);

        retVal.push({ Value: array[0], Split: false });

        if (matchs) {
            for (var i = 0; i < matchs.length; i++) {
                retVal.push({ Value: matchs[i], Split: true });
                retVal.push({ Value: array[i + 1], Split: false });
            }
        }
        return retVal;
    }


    //如果条件为true，则抛出错误。执行该函数的地方，都是不应该出现的错误。
    jv.Check = jv.check = function (condition, msg) {
        if (condition) Alert(msg);
    };

    jv.MyGuid = jv.myGuid = function () {
        var NewRandom = function () {
            return Math.random().toString(16).slice(2);
        };

        var a = NewRandom().slice(0, 8), b = NewRandom().slice(0, 8), c = NewRandom().slice(0, 8), d = NewRandom().slice(0, 8);
        return a + "-" + b.slice(0, 4) + "-" + b.slice(4) + "-" + c.slice(0, 4) + "-" + c.slice(4) + d;
    }

    jv.IsNormalText = jv.isNormalText = function (str) {
        //http://www.cnblogs.com/yukaizhao/archive/2009/01/21/1379411.html
        if (!str) return str;
        if (typeof (str) != "string") return str;

        //去除字母和数字
        //数字区间 [\u0030-\u0039]
        //大写字母区间  [\u0041-\u005A]
        //小写字母区间 [\u0061-\u007A]
        //中文区间  [\u4E00-\u9FBB]

        return str.replace(/([\u0030-\u0039]|[\u0041-\u005A]|[\u0061-\u007A]|[\u4E00-\u9FBB])+/g, "").length;
    };


    jv.Encode = jv.encode = function (value) {
        if (!jv.isNormalText(value)) return value;

        var temp = jv.__Temp_Div_For_Code__;
        if (jv.IsNull(temp)) {
            temp = jv.__Temp_Div_For_Code__ = document.createElement("div");
        }
        (temp.textContent != null) ? (temp.textContent = value) : (temp.innerText = value);
        var output = temp.innerHTML;
        return output;
    }

    jv.Decode = jv.decode = function (html) {
        if (!jv.isNormalText(html)) return html;

        var temp = jv.__Temp_Div_For_Code__;
        if (jv.IsNull(temp)) {
            temp = jv.__Temp_Div_For_Code__ = document.createElement("div");
        }

        temp.innerHTML = html;
        return temp.innerText || temp.textContent;
    }

    jv.GetJsonKeys = jv.getJsonKeys = function (Dict) {
        var ret = [];
        if (jv.IsNull(Dict)) return ret;
        for (var p in Dict) {
            if (!Dict.hasOwnProperty(p)) continue;
            if ($.isFunction(p)) continue;
            ret.push(p);
        }
        return ret;
    }

    //把 Json value 化.处理 Json 的Value 值是 function 的情况. IsRecursion 表示是否递归处理.
    jv.JsonValuer = jv.jsonValuer = function (jsonVaule, valParaCallback, IsRecursion) {
        var ret = {};
        valParaCallback = valParaCallback || $.noop;
        IsRecursion = IsRecursion || false;
        if (!jsonVaule) return ret;
        for (var p in jsonVaule) {
            if (!jsonVaule.hasOwnProperty(p)) continue;
            if ($.isFunction(p)) continue;
            var v = jsonVaule[p];
            if ($.isFunction(v)) {
                var para = valParaCallback(p, v);
                if (para !== false) {
                    ret[p] = v(para);
                    continue;
                }
            }
            ret[p] = v;
        }
        return ret;
    };

    //参考 Boxy._viewport
    jv.GetEyeSize = jv.getEyeSize = function () {
        var w = window;
        //谷歌
        if (!jv.IsNull(w.innerWidth)) return { width: w.innerWidth, height: w.innerHeight };

        var d = document.documentElement;
        if (!jv.IsNull(d.clientWidth)) return { width: d.clientWidth, height: d.clientHeight };

        var b = document.body;
        return { width: b.clientWidth, height: b.clientHeight };
    }

    jv.SetDivJson = function (prefix, JsonValue) {
        jv.boxdy().SetDivJson(prefix, JsonValue);
    }

    //简单判断一个对象是否为空 严格意义上的null ， false,0 不算空.
    jv.IsNull = jv.isNull = function (obj) {
        if (obj === null) return true;
        if (obj === undefined) return true;

        if (obj === false) return false;
        if (obj === 0) return false;
        if (obj === "") return false;

        if (!obj) return true;

        //如果Ie,不好意思,会慢一点.
        if ($.browser.msie /*&& ($.browser.version == 8) 兼容模式的版本号会很低.*/) {
            //采用 try catch 比采用 typeof 效率比是 793/252
            try {
                var d = obj.constructor;
            }
            catch (e) { return "IsNull"; }
            if (/*tc === "undefined" && */(obj.tagName === "shape")) {
                return "IsVml";
                var html = obj.outerHTML;
                if (html.indexOf("<?import") == 0 || html.indexOf("<g_vml_:")) {
                    return "IsVml"; // Vml 也算是Null
                }
            }
        }
        return false;
    }

    //判断一个对象是否有内容项
    jv.HasValue = jv.hasValue = function (obj) {
        if (jv.IsNull(obj)) return false;
        if (obj === "") return false;
        if ($.isArray(obj)) {
            return obj.length > 0;
        }
        else if ($.isPlainObject(obj)) {
            //判断是否是空对象。
            var hasValue = false;
            for (var property in obj) {
                if (!obj.hasOwnProperty(property)) continue;
                hasValue = true;
                break;
            }
            return hasValue;
        }
        return !!obj;
    }

    jv.FindInput = jv.findInput = function (container) {
        var ret = [];

        var cs = container.getElementsByTagName("input");
        for (var i = 0, length = cs.length; i < length; i++) {
            ret.push(cs[i]);
        }

        cs = container.getElementsByTagName("select");
        for (var i = 0, length = cs.length; i < length; i++) {
            ret.push(cs[i]);
        }

        cs = container.getElementsByTagName("textarea");
        for (var i = 0, length = cs.length; i < length; i++) {
            ret.push(cs[i]);
        }

        return ret;
    };

    //取 style 的值，有时还是有用的。
    jv.OriCss = jv.oriCss = function (cssSelector) {
        var obj = this[0];
        if (!obj) return null;
        return obj.style[cssSelector];
    };

    jv.Css = jv.css = function (dom, styleName) {
        var curCSS;
        if (document.documentElement.currentStyle) {
            curCSS = function (elem, name) {
                var left, rsLeft, uncomputed,
                ret = elem.currentStyle && elem.currentStyle[name],
                style = elem.style;

                // Avoid setting ret to empty string here
                // so we don't default to auto
                if (ret === null && style && (uncomputed = style[name])) {
                    ret = uncomputed;
                }

                // From the awesome hack by Dean Edwards
                // http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

                // If we're not dealing with a regular pixel number
                // but a number that has a weird ending, we need to convert it to pixels
                if (!/^-?\d+(?:px)?$/i.test(ret) && /^-?\d/.test(ret)) {

                    // Remember the original values
                    left = style.left;
                    rsLeft = elem.runtimeStyle && elem.runtimeStyle.left;

                    // Put in the new values to get a computed value out
                    if (rsLeft) {
                        elem.runtimeStyle.left = elem.currentStyle.left;
                    }
                    style.left = name === "fontSize" ? "1em" : (ret || 0);
                    ret = style.pixelLeft + "px";

                    // Revert the changed values
                    style.left = left;
                    if (rsLeft) {
                        elem.runtimeStyle.left = rsLeft;
                    }
                }

                return ret === "" ? "auto" : ret;
            };
        }
        else if (document.defaultView && document.defaultView.getComputedStyle) {
            curCSS = function (elem, name) {
                var ret, defaultView, computedStyle;

                name = name.replace(/([A-Z]|^ms)/g, "-$1").toLowerCase();

                if ((defaultView = elem.ownerDocument.defaultView) &&
                (computedStyle = defaultView.getComputedStyle(elem, null))) {
                    ret = computedStyle.getPropertyValue(name);
                    if (ret === "" && !jQuery.contains(elem.ownerDocument.documentElement, elem)) {
                        ret = jQuery.style(elem, name);
                    }
                }

                return ret;
            };
        }

        if (styleName === "cssFloat") {
            styleName = "float";
        }

        return curCSS(dom, styleName);
    };

    //调用： jv.nodeHasClass(node,"query")
    jv.NodeHasClass = jv.nodeHasClass = function (node, className) {
        if (!node) return null;
        if (!className) return null;
        var clss = node.className;
        return (" " + clss + " ").indexOf(" " + className + " ") >= 0;
    };

    jv.GetParentTag = jv.getParentTag = function (node, tag) {
        if (node.tagName == tag) return node;
        if (node.tagName == "BODY") return null;
        return jv.getParentTag(node.parentNode, tag);
    };

    //取下一个节点。排除空文本
    jv.GetNextNode = jv.getNextNode = function (node, nodeFunc) {
        if (!node) return null;
        var next = node.nextSibling;
        if (!next) return null;
        if (next.nodeType == 8) return jv.getNextNode(next, nodeFunc);  //表示 注释comments
        if (next.nodeType == 3) {
            if (!(next.nodeValue || "").trim().length) {
                return jv.getNextNode(next, nodeFunc);
            }
        }
        var ret = nodeFunc ? nodeFunc(next) : true;
        if (!ret) return jv.getNextNode(next, nodeFunc);

        return next;
    };

    //如果 node 是最后一个，则返回 node。
    jv.GetLastNode = jv.getLastNode = function (node, nodeFunc) {
        var next = jv.getNextNode(node, nodeFunc);
        if (!next) return node;
        else return jv.getLastNode(next, nodeFunc);
    };

    //取 childNodes ，排除 空文本。
    jv.GetNodes = jv.getNodes = function (node, nodeFunc) {
        if (!node) {
            return [];
        }
        var child = node.childNodes;
        var ret = [];
        var nodePos = -1;
        for (var i = 0, length = child.length; i < length; i++) {
            var n = child[i];
            if (n.nodeType == 8) continue;  //表示 注释comments

            //表示 文本内容
            if (n.nodeType == 3) {
                if (!(n.nodeValue || "").trim().length) {
                    continue;
                }
            }

            nodePos++;
            var c = nodeFunc ? nodeFunc(n, nodePos) : true;
            if (c) ret.push(n);
        }
        return ret;
    };

    jv.GetNode = jv.getNode = function (node, nodeIndex) {
        if (!node) return null;
        if (!nodeIndex) nodeIndex = 0;

        var child = node.childNodes;
        var ret = [];
        var nodePos = -1;
        for (var i = 0, length = child.length; i < length; i++) {
            var n = child[i];
            if (n.nodeType == 8) continue;  //表示 注释comments

            //表示 文本内容
            if (n.nodeType == 3) {
                if (!(n.nodeValue || "").trim().length) {
                    continue;
                }
            }

            nodePos++;
            if (nodeIndex === nodePos) return n;
        }
        return ret;
    }

    //判断是否是叶子节点。
    jv.IsLeaf = jv.isLeaf = function (node) {
        if (!node) {
            return null;
        }
        var child = node.childNodes;
        var ret = true;
        for (var i = 0, length = child.length; i < length; i++) {
            var n = child[i];
            if (n.nodeType == 8) continue;  //表示 注释comments

            //表示 文本内容
            if (n.nodeType == 3) {
                continue;
            }
            return false;
        }
        return ret;
    };

    //判断两个控件的隶属关系。
    jv.InContainer = jv.inContainer = function (node, parentNode) {
        if (!node) return false;
        if (node.parentNode == parentNode) return true;
        return jv.inContainer(node.parentNode, parentNode);
    };

    jv.CreateEventSource = jv.createEventSource = function (obj, evObj) {
        if (obj.originalEvent) return;

        var evTag = evObj || jv.GetDoer();
        if ($.browser.msie && $.browser.version < 7) {
        }
        else {
            window.eventTarget = obj.eventTarget = obj.target = evTag;
            obj.originalEvent = true;
        }
    }

    //取 事件发生源. 尽量传入 event 提高性能。(返回的非JQeury对象)
    jv.GetDoer = jv.getDoer = function (evt, IsUseWindowEvent, func) {
        var maxDeep = 60;

        if ($.isFunction(evt)) { func = evt; evt = null; }

        if (IsUseWindowEvent) evt = evt || window.event;

        if (!evt) {
            var findUpEvent = function (args) {

                if (maxDeep < 0) {
                    if (window.event) return window.event;
                    else return true; //return true 之后， 执行 if (_jev) { return _jev; } 不再递归。
                }
                maxDeep--;
                if (args && args.callee.caller && args.callee.caller.arguments) {
                    var _jev = false;
                    var theArgs = args.callee.caller.arguments;
                    for (var i = theArgs.length - 1; i >= 0; i--) {
                        var d = theArgs[i];
                        if (_jev) break;
                        if (d) {
                            if (d.originalEvent) {
                                if (func && (func(d) === false)) { continue; }
                                _jev = d;
                                break;
                            }
                            else if (d.target) {
                                var dtype = d.type;
                                if (dtype == "readystatechange") continue;  //修正 Chrome浏览器.XHR , 有XMLHttpRequestProgressEvent
                                if (func && (func(d) === false)) { continue; }
                                _jev = d;
                                break;
                            }
                            else if (d.srcElement) {
                                if (d.type == "readystatechange") continue;  //修正 Chrome浏览器.XHR , 有XMLHttpRequestProgressEvent
                                if (func && (func(d) === false)) { continue; }
                                _jev = d;
                                break;
                            }
                            _jev = false;
                        }
                    }

                    if (_jev) { return _jev; }
                    else { return findUpEvent(theArgs); }
                }
                else if (window.event) {
                    return window.event;
                }
            };
            evt = findUpEvent(arguments);
        }

        if (evt) {
            maxDeep = 60;

            var findInDeep = function (obj) {
                if (maxDeep < 0) {
                    return obj;
                }
                maxDeep--;

                if (obj.tagName == "A") return obj;

                var src = obj.srcElement
                || obj.target
                || (obj.originalEvent ? (obj.originalEvent.srcElement || obj.originalEvent.target) : null)
                if (src) return findInDeep(src);
                else return obj;
            };

            return findInDeep(evt)
            || (window.event && window.eventTarget);  // 获取触发事件的源对象,注:当是IE时,Event是只读的,EventTarget 作为临时替代方案.

            return src;
        }
        else return window.event && window.eventTarget;
    }

    jv.LoadJsCss = jv.loadJsCss = function (FileType, FileName, callback) {
        if (!FileType || !FileName) return;


        var fileref, bindLoadedCallBack,
    check = function (findFile, selector, attr) {
        var IsHas = false;
        $(selector).each(function () {
            var self = $(this);
            if (self.attr(attr) == findFile) { IsHas = true; return false; }
            else if (self.attr(attr) == findFile) { IsHas = true; return false; }
        });
        return IsHas;
    };

        if (!callback) {
            callback = bindLoadedCallBack = $.noop;
        }
        else {
            bindLoadedCallBack = function (element) {
                element.onreadystatechange = element.onload = function () {
                    if (!$.browser.msie || this.readyState == 'loaded' || this.readyState == 'complete') {
                        this.onreadystatechange = this.onload = null;
                        callback();
                    }
                };
            };
        }
        if (FileType == "js") {
            if (check(FileName, "script", "src") == true) return callback();
            //判断文件类型
            fileref = document.createElement('script'); //创建标签
            $(fileref).attr("type", "text/javascript") //文件的地址
            bindLoadedCallBack(fileref);
            $(fileref).attr("src", FileName);
        }
        else if (FileType == "css") {
            if (check(FileName, "link", "href") == true) return callback();

            //判断文件类型
            fileref = document.createElement("link");
            fileref.rel = "stylesheet";
            fileref.type = "text/css";
            bindLoadedCallBack(fileref);
            fileref.href = FileName;
        }
        else if (FileType == "style") {
            fileref = document.createElement("style");
            $(fileref).attr("type", "text/css");

            if (fileref.styleSheet) {
                fileref.styleSheet.cssText = FileName;
            } else {
                $(fileref).html(FileName);
            }
            callback();
        }
        else if (FileType == "deferjs") // defer javascript
        {
            if (!$(FileName).length) return;
            fileref = document.createElement('script'); //创建标签
            $(fileref).attr("type", "text/javascript").text($(FileName).html());
            callback();
        }
        else return;

        if (jv.IsNull(fileref) == false) {
            if ($.browser.msie) {
                $("head")[0].appendChild(fileref);
            }
            else {
                $("head").append(fileref);
            }
            //document.getElementsByTagName("head")[0].appendChild(fileref);
        }
    };

    jv.GetPageScroll = jv.getPageScroll = function () {
        var x, y;
        if (window.pageYOffset) {
            // all except IE
            y = window.pageYOffset;
            x = window.pageXOffset;
        } else if (document.documentElement && document.documentElement.scrollTop) {
            // IE 6 Strict
            y = document.documentElement.scrollTop;
            x = document.documentElement.scrollLeft;
        } else if (document.body) {
            // all other IE
            y = document.body.scrollTop;
            x = document.body.scrollLeft;
        }
        return { X: x, Y: y };
    }

    jv.AddToFavorite = jv.addToFavorite = function (url, title, desc) {
        // 加入收藏夹
        if (document.all) {
            window.external.addFavorite(url, title, desc);
        }
        else if (window.sidebar) {
            window.sidebar.addPanel(title, url, "");
        }
    };

    jv.Random = jv.random = function () {
        return Math.random().toString(36).slice(2);
    };
    //弹出,如果应用弹出，尽量调用 jv.PopList 和 jv.PopDetail（普通调用亦可）
    jv.Pop = jv.pop = function () {
        var Url, option;
        if (arguments.length >= 2) {
            Url = arguments[0];
            option = arguments[1];
        }
        else if (arguments.length == 1) {
            option = arguments[0];
            Url = option["url"];
        }
        else { return alert("弹出需要参数"); }

        var p = option || {};
        var Width = p.width,
            Height = p.height,
            name = p.name || p.entity,
            callback = p.callback || p.loadCallback;


        if (p.openMode) {
            var m = p.openMode + "";
            if (m.indexOf("Boxy:") == 0) {
                Boxy.load(Url, { filter: m.substr(5), modal: true, title: "查看详情", width: Width });
                return;
            }
        }


        var top = 0, left = 0, winBarHeight = 80;
        if (Height < 0) {
            //作修正
            if (Height < 0 - screen.height / 2) {
                Height = 0 - screen.height / 2;
            }

            top = 0 - Height;
            Height = screen.height - winBarHeight - 2 * top;
        }
        else if (Height > 0) {
            top = (screen.height - winBarHeight - Height) / 2;
        }
        else {
            top = 50;
            Height = screen.height - winBarHeight - 100;
        }

        if (Width < 0) {
            //作修正
            if (Width < 0 - screen.width / 2) {
                Width = 0 - screen.width / 2;
            }

            left = 0 - Width;
            Width = screen.width - 2 * left;
        }
        else if (Width > 0) {
            left = (screen.width - Width) / 2;
        }
        else {
            left = 200;
            Width = Width || screen.width - 400;
        }

        var autoFocus = true;
        var jvUrl = jv.url(Url);

        var useModal = !p.openMode;

        if (jvUrl.attr("UseModalDialog")) { useModal &= $.browser.msie; }
        else if (jvUrl.attr("UseWindowOpen")) { useModal = false; }
        else if (jvUrl.href.toLowerCase().indexOf("reportweb/") >= 0) {
            autoFocus = false;
            useModal = false;
        }

        if (jv.IsNull(p.autoFocus) == false) autoFocus = p.autoFocus;

        window.popArg = { left: left, top: top, width: Width, height: Height };

        if (useModal) {
            var sFeature = [];

            sFeature.push("dialogTop=" + top + "px");
            sFeature.push("dialogHeight=" + Height + "px");
            sFeature.push("dialogLeft=" + left + "px");
            sFeature.push("dialogWidth=" + Width + "px");
            sFeature.push("center=yes");
            sFeature.push("resizable=yes");

            window._PopCallback_ = callback;

            Url = jvUrl.attr("_", jv.Random()).toString();

            window.showModalDialog(Url, window, sFeature.join(";"));
            return;
        }
        else {
            if (jv.getType(p.openMode) == "string") {
                name = p.openMode;
            }
            else {
                name = name || "jvPop";
            }

            var pWin = window, xmlDoc;
            var sFeature = [];


            sFeature.push('height=' + Height + 'px');
            sFeature.push('width=' + Width + 'px');
            sFeature.push('top=' + top + 'px');
            sFeature.push('left=' + left + 'px');
            sFeature.push('toolbar=no');
            sFeature.push('menubar=no');
            sFeature.push('scrollbars=yes');
            sFeature.push('resizable=yes');
            sFeature.push('location=no');
            sFeature.push('status=no');

            //返回一个 window 对象。
            xmlDoc = window.open(Url, name, ["_blank", "_self", "_parent", "_top"].indexOf(name) >= 0 ? null : sFeature.join(","));

            if (!xmlDoc) {
                Alert("浏览器阻止了弹出窗口!");
                return;
            }

            var bindEvent = function (callback) {
                $(xmlDoc.document).ready(function () {
                    $(pWin).bind("click", function () { xmlDoc.focus(); });

                    try {
                        if (callback) { callback(xmlDoc, xmlDoc.document); }
                    }
                    catch (e) { }

                    //ie
                    /*如果xmlDoc 关掉的话.可能会报 拒绝访问 */
                    $(pWin.document).bind("click",
                                function () {
                                    /*如果xmlDoc 关掉的话.可能会报 拒绝访问 */
                                    try {
                                        if (xmlDoc.document && xmlDoc.document.focus) xmlDoc.document.focus();
                                    } catch (e) {
                                        pWin.document.onclick = null;
                                    }

                                }
                            );
                });
            };

            if (autoFocus) {
                bindEvent(callback);
            }

            return xmlDoc;
        }
    }


    //兼容FF,IE
    jv.ExecJs = jv.execJs = function (Js, target, params) {

        if (!Js) return;
        //    window.execScript ? execScript(Js) : eval.call(null, Js);
        target = target || jv.GetDoer(null, false);
        params = params || "";
        //Ajax 回来之后， 事件源已经确定(xhrEvent). 清除 window.eventTarget.
        if (window.event) {
            window.eventTarget = null;
        }
        with (window)
            return eval("eval(function(" + params + "){ " + Js + "})")({ originalEvent: true, target: target });
    };

    //返回值均为字符串： null,undefined ,number,string,array,boolean,date,error,function,math,regexp,object,json,htmldom(也可能返回具体的类名)
    jv.GetType = jv.getType = function (object) {
        //var _t;
        //return ((_t = typeof (object)) == "object" ? object == null && "null" || Object.prototype.toString.call(object).slice(8, -1) : _t).toLowerCase();

        /* 
            Object.prototype.toString.call(object)  和  object + ""  在IE8 下不一样！  Dom对象在前者会返回 [object] or  [object object],而后者返回 [object htmltablerowelement]
            但   Object.prototype.toString.call(object)  和  object + ""  在IE7 下是一样的。
            为了浏览器的一致性。   不返回具体的 类名。     
                */
        if (object === null) {
            return "null";
        }
        else if (object === undefined) return "undefined";

        var _t = (typeof (object)).toLowerCase();
        if (_t == "object") {
            _t = Object.prototype.toString.call(object).toLowerCase().slice(8, -1) || _t;
            //            var _ot = Object.prototype.toString.call(object).toLowerCase() ;
            //            if ( _ot.indexOf ("[object ") === 0 ){
            //                _ot = object + "";

            //                
            //                if ( _ot == "[object]"){
            //                    //如果使用 + 又回去了。
            //                }

            //                return _ot.toLocaleLowerCase().slice(8, -1) || _t;
            //            }
        }

        //借鉴 $.isPlanObject
        if (_t == "object") {
            if (object.nodeType || ("setInterval" in object)) return "htmldom";
        }
        return _t;
    };


    // jv.url() 返回的调用的是 jv.url(jv.page().href)  可能和  jv.url(window.location) 返回值不同. 例如静态化之后.
    jv.Url = jv.url = function (url) {
        var proc = function (href) {
            //返回对象。Jquery Json Url
            var jurl = {};
            jurl.root = jv.Root;
            jurl.href = href;
            jurl.kv = {};
            jurl.search = "";

            if (jurl.href === "") {
                jurl.root = jv.Root;
            }
            else {
                var ind, isProtocol = jurl.href.indexOf("://");
                if (isProtocol > 0) {
                    ind = jurl.href.indexOf("/", isProtocol + 3);
                    jurl.root = jurl.href.substr(0, ind) + jv.Root;
                }
                else if (jurl.href.indexOf("~/") == 0) {
                    jurl.href = jv.Root + jurl.href.slice(2);
                }
            }

            jurl.attr = function (key, value) {
                if ($.isPlainObject(key)) {
                    var queryJson = key;
                    $.each(jv.GetJsonKeys(queryJson), function (i, d) {
                        jurl.attr(d, queryJson[d]);
                    });
                    return jurl;
                }
                else if (jv.IsNull(value) == false) {
                    jurl.kv[key] = value;
                    return jurl;
                }
                else return jurl.kv[key];
            }
            jurl.removeAttr = function (key) {
                delete this[key];
                return this;
            }
            jurl.tohref = function () {
                if (this.href === "") return "";

                var retVal = this.url,
                    queryAry = [];
                $.each(jv.GetJsonKeys(this.kv), function (i, d) {
                    if (jv.IsNull(d)) return;
                    queryAry.push(encodeURIComponent(d) + "=" + (jv.IsNull(jurl.kv[d]) ? "" : encodeURIComponent(jurl.kv[d])));
                });
                return retVal + (queryAry.length > 0 ? "?" : "") + queryAry.join("&");
            }
            jurl.toString = jurl.tohref;
            if (!jurl.href) return jurl;
            var seg1 = jurl.href.indexOf("?");
            if (seg1 >= 0) {
                jurl.url = jurl.href.slice(0, seg1);
                jurl.search = jurl.href.slice(seg1);
                var seg2 = jurl.search.slice(1);
                if (seg2) {
                    var seg3 = seg2.mySplit('&', true);
                    $.each(seg3, function () {
                        var seg4 = this.mySplit('=', true),

                    key = decodeURIComponent(seg4[0]),
                    value = jv.IsNull(seg4[1]) ? "" : decodeURIComponent(seg4[1]);
                        jurl.kv[key] = value;
                    });
                }
            }
            else { jurl.url = jurl.href; }

            return jurl;
        };
        if (url === "") {
            return proc("");
        } else if (url) {
            var typ = jv.getType(url);
            if (typ == "string") return proc(url);
            else if (typ == "location") { return proc(url.toString()); }
            else return proc(jv.page(url).href);
        }
            //    else if (jv.boxdy()[0] == document) {
            //        return proc(window.location.href);
            //    }
        else return proc(jv.page().href || "");
    };


    /*主要针对 Boxy 打开远程Url 加载 jv.Request 的问题。
    在Dom树上的每一个Boxy以及LoadView容器上，设置形成 jvReqeust 数据的导级结构, 类jvRequest 有利于查找.
    依然保留 jv.Request , 表示最执行的 Reqeust 对象.
    由于有作用域, jv.AddRequest 与 其它 Ajax页面互斥. 如 jQuery.ui.Tabs.

    三种情况: 一种是页面加载的时候,另一种是 Boxy.Load,3是 $("#").LoadView
    */
    jv.AddRequest = jv.addRequest = function (request) {
        //    request.originalEvent = true;

        jv.Request = request;
        var container = $(".jvRequest_waite");
        if (container.length <= 0) {
            container = $(document);
        }

        container.addClass("jvRequest").data("jvRequest", request).removeClass("jvRequest_waite");
        jv.AllRequest.push(jv.Request);
    }

    //开发者禁止使用 jv.Request.并且尽量不使用 jv.GetRequest ，而使用它的别名: jv.page()
    //jv.page() 下本系统使用的当前 《作用域页面》 变量及函数 
    //jv.page().unload 回调函数字典.  用于卸载当前《作用域页面》 时执行的回调. 
    //jv.page().Keys 表示当前《作用域页面》 Mvc 中 Route 中的每个部分,及 URL 参数的每个部分.
    //jv.page().area,controller,action,uid  表示当前《作用域页面》 Mvc 中 Route 中的每个部分,及 URL 参数
    //jv.page().PowerJsonResult  表示从 权限系统中获取的当前《作用域页面》的权限集
    //jv.page().entity  表示 当前《作用域页面》的主实体
    //jv.page().href 表示当前《作用域页面》的 URL
    //jv.page().originalEvent
    //jv.page().target 表示当前 《作用域页面》 的传递的事件源
    jv.Page = jv.page = jv.GetRequest = jv.getRequest = function (con) {
        var src = con || jv.GetDoer(con);

        var retVal = null;
        if (src && src != document) {
            var jsrc = $(src);
            retVal = jsrc.data("jvRequest") || jsrc.closest(".jvRequest").data("jvRequest") || jv.AllRequest[0];
        }
        else retVal = jv.AllRequest[0];

        //    if (retVal.boxy && retVal.boxy.data("boxy") && retVal.boxy.data("boxy").options["tempDialog"]) {
        //        return jv.page(retVal.boxy.data("srcForBoxy"));
        //    }

        /* 让总页面调用子页面成为可能,调用方式:
        var page = jv.page($("#Con")) ;
        page.Edit_OK(page) ;
        */

        if (!retVal) return jv;

        retVal.originalEvent = true;
        retVal.target = src;
        return retVal;
    }

    //取当前Boxy，如果没有，返回当前 Boxy。 boxdy 是 Boxy 和 Body 的合成。可称做 boxy-body .它返回的是 jQuery对象.
    //找容器.返回容器对象. 
    // 该函数会寻找 控件源是 Con 所在的 容器。 当容器是 document 时， 返回 doc 或 document
    jv.Boxdy = jv.boxdy = function (con, doc) {
        //    if (con && con.originalEvent) con = jv.GetDoer(con);
        var src = con || jv.GetDoer(con), request, bxdy = [], jsrc = $(src);

        if (src && src != document) {
            request = jsrc.data("jvRequest");

            if (!request) {
                jsrc = jsrc.closest(".jvRequest");
                request = jsrc.data("jvRequest");
            }

            if (request) {
                bxdy.push((request.boxy || jsrc)[0]);

                //如果是 Boxy 内，则把 Boxy 的源也加上。
                if (jsrc.data("boxy") && jsrc.data("boxy").options["tempDialog"] && jsrc.data("srcForBoxy")) {
                    bxdy.push(jv.boxdy(jsrc.data("srcForBoxy"))[0]);
                }

                return $(bxdy);
            }
        }
        return $(doc || document);
    }

    jv.ParentBoxdy = jv.parentBoxdy = function (con) {
        return jv.boxdy(jv.boxdy(con).parents(".jvRequest:first"));
    };

    jv.DeleteObj = jv.deleteObj = function (obj, deepth) {
        if (!obj) return;
        deepth = deepth || 0;
        try {
            for (var p in obj) {
                if (deepth < 3) {
                    var val = obj[p];
                    var type = jv.getType(val);
                    if (type == "array") {
                        for (var i = 0; i < val.length ; i++) {
                            jv.DeleteObj(val, deepth + 1);
                        }
                    }

                    if (",null,undefined,number,string,boolean,date,error,function,math,regexp,htmldom,".indexOf("," + type + ",") < 0) {
                        jv.DeleteObj(obj[p], deepth + 1);
                    }
                }

                delete obj[p];
            }
        } catch (e) { }
    };

    jv.UnloadRequest = jv.unloadRequest = function () {
        if (jv.AllRequest.length > 1) {
            var PinThis = false;
            if (jv.page() == jv.AllRequest[jv.AllRequest.length - 1]) {
                PinThis = true;
            }

            if (jv.page().unload) {
                var unloadObj = jv.page().unload;
                for (var unloadFunc in unloadObj) {
                    if (!unloadObj.hasOwnProperty(unloadFunc)) continue;

                    if (!unloadFunc) continue;
                    var func = unloadObj[unloadFunc];
                    if (func && $.isFunction(func)) {
                        func();
                    }
                }
            }

            if (jv.AllRequest.length > 1) {

                //            if ($.browser.msie) {
                //                //http://fogtower.iteye.com/blog/617997
                //                $("form,button,input,select,textarea,a,img", jv.boxdy()).unbind().removeData();
                //                //,table,tr,td
                //                CollectGarbage();
                //            }



                //            delete jv.Request.target;
                //            delete jv.Request.srcElement;
                //            delete jv.Request.eventTarget;

                jv.AllRequest.pop();
                jv.Request = jv.AllRequest[jv.AllRequest.length - 1];
            }

            if (PinThis == false) {
                jv.UnloadRequest();
            }
        }
    };

    jv.RegisteUnload = jv.registeUnload = function (key, callback) {
        jv.page().unload = jv.page().unload || {};
        jv.page().unload[key] = callback;
    };

    //data : {button:[],edit:[],view:[]}
    //操作项，修改项，查看项。 FlexiGrid ，Card 专用扩展。
    //以 ] 开始的是 Grid 是各列。
    jv.TestRole = jv.testRole = function (option) {
        var setting = $.extend({ grid: false, card: false, callback: false }, option), pageGrid = jv.boxdy().find(".myGrid");

        if (pageGrid.length == 1) {
            setting.grid = pageGrid[0].grid;
        }

        var grid = setting.grid, url = jv.Root + "TestListPower.ashx", entity = [];


        var ProcRole = function (res) {
            if (jv.IsNull(res) || res.msg == "SysAdmin") {
                if (setting.callback) setting.callback(res);
                return;
            }
            if (res.msg) {
                document.clear();
                document.writeln(res.msg);

                if (setting.callback) setting.callback(res);
                return;
            }

            //start.
            if (jv.IsNull(res.button) == false) {
                //$(".fbutton", grid.gDiv).filter(function (_i, _d) { return $.inArray("(" + $(_d).text() + ")", res.button) == -1 })
                var btns = [];
                $.each(res.button, function (i, btn) {
                    //规则 : 以 ] 开始的是 Grid 是各列.
                    var col1 = btn.slice(0, 1);
                    var colName = btn.slice(1);
                    if (col1 == "]") {
                        grid.toggleCol(colName, false);
                    }
                    else {
                        btns.push(btn);
                    }
                });

                $.each(btns, function (i, btnSelector) {
                    $(btnSelector).each(function (_i, d) {
                        var self = $(d);
                        if ($.inArray(d.tagName, "INPUT", "BUTTON", "SELECT") >= 0) {
                            self.attr("disabled", "disabled");
                        }
                        //                        else {
                        //                            self.css("color", "gray");
                        //                        }
                        self.unbind().hide();
                    });
                });

            }

            if (setting.callback) setting.callback(res);

            return;
        };

        if (jv.page().PowerJsonResult) {
            ProcRole(jv.page().PowerJsonResult);
            return;
        }

        if (grid) {
            var selfEntity = $(grid.p.colModel)
                        .filter(function () { return this && (jv.IsNull(this.name) == false) && this.name.indexOf(".") > 0; })
                        .out(function () { return this.name.mySplit('.', true)[0]; });

            for (var i = 0, len = selfEntity.length ; i < len ; i++) {
                var ent = selfEntity[i];
                if (ent) {
                    entity.push(ent);
                }
            }
        }
        //    else {
        //        entity.push($("input[").
        //    }

        //如果没有返回的数据,或返回数据没有定义实体,则忽略
        //    if (!grid.data || !grid.data.entity) return;

        var mainEntity = (grid && grid.data && grid.data.entity) || (grid && grid.p.role.entity) || jv.page().entity;

        //jv.page().entity 用于 Card 权限校验,当有多个实体时, 用"," 分隔.
        if (mainEntity) {
            $.merge(entity, mainEntity.mySplit(',', true));
        }

        entity.push(jv.page()["controller"]);

        entity = $.unique(entity);

        $.post(url, { area: jv.page()["area"], controller: jv.page()["controller"], action: jv.page()["action"], entity: entity.join(',') }, function (res) {
            jv.page().PowerJsonResult = res;

            ProcRole(jv.page().PowerJsonResult);
        });

    };


    //自定义过滤，用于从其它页面过滤内容。
    jv.MyFilter = jv.myFilter = function (html, filter) {
        if (filter) {
            var fh = html.filter(filter);
            if (!fh.length) {
                fh = html.find(filter);
            }

            return fh;
        }
        else {
            //取Boxy
            html = html.filter(function () {
                //如果是文本, 这里是指空白.
                var one = this;
                if (one.nodeType == 3) return false;
                if (one.tagName == "SCRIPT") return false;
                if (one.tagName == "STYLE") return false;
                if (one.tagName == "LINK") return false;
                return true;
            }).last();
        }
        return html;
    }

    //Ajax加载页面后，处理页面Header内容。Boxy用。
    jv.ExecHtml = jv.execHtml = function ($html, callIn, src) {
        $html.filter("link").each(function () { jv.LoadJsCss("css", $(this).attr("href")); });
        $html.filter("style").each(function () { jv.LoadJsCss("style", $(this).html()); });

        if (callIn) callIn();

        $html.filter(function () { return this.src; }).each(function () { jv.LoadJsCss("js", $(this).attr("src")); });

        //jv.target = src;
        $html.filter("script").filter(function () { return !this.src; }).each(function () { jv.ExecJs($(this).html(), src); });

        jv.MyOnLoad();
    }

    jv.IsInBoxy = jv.isInBoxy = function (ev) {
        return $(jv.GetDoer()).closest(".boxy-wrapper").length > 0;
    }

    jv.Hide = jv.hide = function (callback) {
        var boxy = window.Boxy && Boxy.getOne();

        if (boxy) {
            boxy.hide(callback);
            return true;
        }
        else if (window.opener != null) {
            if (callback) { callback(); }
            window.close();
            return true;
        }
        return false;
    }



    //该方法是Edit页面的第二个执行的方法,之前是 TextHelper 方法.
    jv.SetDetail = jv.setDetail = function (options) {
        var p = $.extend({ callback: false, detail: "Detail", container: jv.boxdy() }, options);

        $($(".Main:last", p.container)[0] || p.container).SetDetail(p);
    }


    jv.PopDetail = jv.popDetail = function (setting, ev) {
        if (ev && ev.stopPropagation) { ev.stopPropagation(); }
        var p = { area: "", entity: "", width: false, height: false, url: "", data: {}, detail: "Detail", openMode: false, loadCallback: false };
        p = $.extend(p, setting);

        p.obj = jv.GetDoer();
        p.data.code = p.data.code || jv.GetKeyHidden(p).val();

        if (p.url) {
            p.url = jv.url(p.url);

            if (p.url.root && (p.url.url.slice(p.url.root.length - 1, 1) == "/")) {
                var sects = p.url.url.slice(p.root.length).mySplit(/\/|\\/, true);
                p.area = p.area || sects[0];
                p.entity = p.entity || sects[1];
                p.list = p.list || sects[2];
            }
        }
        else if (!p.data.code) {
            return;
        }

        if (!p.entity) { p.entity = jv.page().controller; }

        if (!p.url) {
            p.url = jv.url(jv.Root + (p.area + "/" || "") + p.entity + "/" + p.detail + "/" + p.data.code + ".aspx");
        }
        if (p.query) {
            p.url.attr(p.query);
        }


        var whConfig = jv.PopDetailConfig(p.area, p.entity, p.detail);

        if (!p.width) {
            p.width = whConfig[0];
        }

        if (!p.height) {
            p.height = whConfig[1];
        }

        p.openMode = whConfig[2];

        jv.Pop(p.url.tohref(), p);
    }

    //取 Key控件, 参数是 Kv 的任意对象.
    jv.GetKeyHidden = jv.getKeyHidden = function (p) {
        if (p.key) {
            if ($(p.key)[0].tagName.toLowerCase() == "input")
                return $(p.key);
            else {
                return $(p.key).find("input[type=hidden]:first");
            }
        }

        var obj = p.obj;
        var jobj = $(obj), hiden;

        //    if (jobj.hasClass("key")) {
        //        hiden = jobj.find("input[type=hidden]:first");
        //        if (hiden.length > 0) return hiden;
        //    }


        //    if (jobj.hasClass("val")) {
        //        hiden = jobj.prev(".key:first").find("input[type=hidden]:first");
        //        if (hiden.length > 0) return hiden;
        //        else {
        //            hiden = jobj.find("input[type=hidden]:first");
        //            if (hiden.length > 0) return hiden;
        //        }
        //    }


        hiden = jobj.closest(".key").find("input[type=hidden]:first");
        if (hiden.length > 0) return hiden;


        hiden = jobj.closest(".val").prev(".key:first").find("input[type=hidden]:first");
        if (hiden.length > 0) return hiden;

        hiden = jobj.closest(".val").find("input[type=hidden]:first");
        if (hiden.length > 0) return hiden;


        //下找
        hiden = jobj.find("input[type=hidden]:first");
        if (hiden.length > 0) return hiden;


        //同辈找
        hiden = jobj.siblings("input[type=hidden]:first");
        if (hiden.length > 0) return hiden;

        return $();
    };

    //取显示控件 , 参数是 Kv 的任意对象.
    jv.GetValueDisplay = jv.getValueDisplay = function (p) {
        if (p.val) return $(p.val);
        if (!p.obj) p.obj = jv.GetDoer();

        var obj = p.obj;
        var jobj = $(obj), dis;

        //    if (jobj.hasClass("key")) {
        //        dis = jobj.next(".val");
        //    }
        //    else if (jobj.hasClass("val")) {
        //        dis = jobj;
        //    }
        //    else {
        //    }

        var key = jobj.closest(".key");
        if (key.length > 0) {
            dis = key.next(".val");
        }
        else dis = jobj.closest(".val");


        if (dis.length > 0) {
            var dis_con = dis.find(".Pin,.pin,.ref").first();
            if (dis_con.length) return dis_con;


            dis_con = dis.find("input[type=text]:visible");
            if (dis_con.length) { return dis_con; }
            dis_con = dis.find("textarea:visible");
            if (dis_con.length) { return dis_con; }

            else return dis;
            //        var child = dis.children();
            //        if (child.length == 0) 
            //        else return dis.children().first();
        } else {
            if (jobj.hasClass("ref")) return jobj;
        }

        return $();
    };

    //取显示控件的值 , 参数是 Kv 的任意对象.
    jv.GetDisplayValue = jv.getDisplayValue = function (p) {
        var obj = p.obj;
        var jobj = jv.GetValueDisplay(p);
        if (!jobj || !jobj.length) return "";

        if ($.inArray(obj.tagName, ["INPUT", "TEXTAREA", "SELECT"]) >= 0) return jobj.val().trim();
        else if (jobj.children().length) {
            var subInput = jobj.find("input").filter(function () { return !$(this).is(":hidden"); });
            if (subInput.length) return subInput.val();
            else {
                p.obj = jobj.children()[0]; return jv.GetDisplayValue(p);
            }
        }
        else return jobj.text().trim() || "";
    };

    //设置显示控件的值 , 参数是 Kv 的任意对象.
    jv.SetDisplayValue = jv.setDisplayValue = function (p, Keys, Values) {
        var jobj = jv.GetValueDisplay(p);
        if (!jobj || !jobj.length) return jobj;
        var obj = jobj[0];

        Keys = Keys || [];
        Values = Values || [];

        var title;


        if (jobj && jobj.length) {
            title = Values.join(",");
            jobj.val(title);
        }

        if (false) {
            var cons = [];

            var doer = jv.GetDoer(), onlyList;
            for (var i = 0; i < Keys.length; i++) {
                if (Keys[i]) {
                    if (doer) {
                        if (doer.ownerDocument.defaultView) {
                            onlyList = doer.ownerDocument.defaultView.jv.page().OnlyList
                        }
                        else if (doer.ownerDocument.parentWindow) {
                            onlyList = doer.ownerDocument.parentWindow.jv.page().OnlyList;
                        }
                    }

                    if (onlyList !== false) {
                        cons.push(Values[i]);
                    }
                    else {
                        cons.push("<span class='Link' onclick=\"jv.PopDetail({area:'{1}',entity:'{2}',data:{code:'{3}'}},event);return false;\">{0}</span>"
                    .format(Values[i], p.area, p.entity, Keys[i]));
                    }
                }
                else {
                    cons.push("(空)");
                }
            }

            title = cons.join(",") || "(空)";
            jobj.html(title);
            title = jobj.text();
        }

        jobj.attr("title", title);
        return jobj;
    };

    //弹出指定Area，Controller 的List页面。
    //默认选择对象后，点击该对象可弹出卡片 ；如果该 Controller 没有定义卡片，可在列表页面调用 jv.page.OnlyList = true。
    jv.PopList = jv.popList = function (setting, ev) {
        var p = {
            area: "",                           //表示弹出Mvc页面的 Area
            entity: "",                         //表示弹出Mvc页面的 Controller
            list: "List",                       //表示弹出Mvc页面的 列表的Action名
            detail: "Detail",                   //弹出 Detail 的Action
            uid: "",                            //表示弹出Mvc页面的 uid 参数名。
            query: {},                          //表示弹出Mvc页面的Url Query , Json Style data .
            url: "",                            //指定弹出的 Url。如果指定，则不使用Mvc各部分。
            loadCallback: false,
            callback: false,                    //用户选择列表项，点确定后的回调。
            width: false,                       //弹出窗口宽度
            height: false,                      //弹出窗口高度
            mode: "radio",                      //选择模式， mode:radio,check,none,单选，多选，不选。
            data: { code: "", name: "" },       //指定默认值。给出 code 和 name 
            key: false,                         //隐藏值选择器，参见： jv.GetHiddenDisplay 。如果该选择器对象是 input，则返回该选择器对象，否则，以该选择器为容器，在内部对 hidden 进行查找。
            val: false,                         //显示值选择器，参见： jv.GetValueDisplay 。如果该选择器对象是 input，则返回该选择器对象，否则，以该选择器为容器，在内部对 hidden 进行查找。
            dataSource: false                   //设置弹出页面的数据源。参见：flexigrid.p.DataSource
        };
        p = $.extend(p, setting);

        if (p.url) {
            p.url = jv.url(p.url);

            if (p.url.root && (p.url.url.slice(p.url.root.length - 1, 1) == "/")) {
                var sects = p.url.url.slice(p.root.length).mySplit(/\/|\\/, true);
                p.area = p.area || sects[0];
                p.entity = p.entity || sects[1];
                p.list = p.list || sects[2];
            }
        }


        if (!p.area) {
            p.area = jv.page()["area"];
        }

        if (!p.entity) {
            p.entity = jv.page().controller;
        }

        p.obj = jv.GetDoer();

        //        //如果 不合规范,只弹Detail
        //        var IsOnlyView = function () {

        //            var action = (jv.page().action || "").toLowerCase(),    //action 必有值.
        //            list = (jv.page().list || p.list).toLowerCase(),
        //            detail = (jv.page().detail || p.detail).toLowerCase(),
        //            onlyView = !action || (action == detail) || (list == detail);

        //            if (!onlyView) {
        //                onlyView = $(p.obj).closest(".jvDetail").data("jvDetail");
        //            }
        //            return onlyView;
        //        }


        //        if (IsOnlyView()) {
        //            return;
        //        }


        if (!p.data.code) {
            p.data.code = jv.GetKeyHidden(p).val() || "";
            p.data.name = jv.GetDisplayValue(p) || ""; // $(p.obj).next().find(">span").text();
        }

        var GetRefValue = function (data, role) {
            if (!role || !role.id) return {};
            if (jv.IsNull(data) || $.isEmptyObject(data)) return {};

            var retVal = {};

            if (role.id in data) {
                retVal["id"] = data[role.id];
            }
            else if ("id" in data) {
                retVal["id"] = data["id"];
            }


            if (role.name in data) {
                retVal["name"] = data[role.name];
            }
            else if ("name" in data) {
                retVal["name"] = data["name"];
            }

            //不应该出现多name 的状态。
            //            else if ( role.name.indexOf(",") >=0 ){
            //                var nameValue = [];
            //                $.each(role.name.mySplit(',', true), function () { nameValue.push(data[this]); });
            //                retVal["name"] = nameValue.join(",") ;
            //            }
            return retVal;
        }

        if (p.dataSource) {
            window.DataSource = p.dataSource;
        }

        if (p.mode == "radio") {
            window._Ref_Callback_ = function (role, data, grid, evt) {
                if (jv.IsNull(data)) data = [{}];
                if (jv.IsNull(role)) role = { id: "", name: "" };
                if (!data.length) data = [{}];

                var val = GetRefValue(data[0], role);
                if (!val.id) val.name = "(空)";

                var keyCon, valCon;

                if (p.refClick &&
                    (p.refClick(role, data, grid, { originalEvent: true, target: p.obj }) === false)) {
                    keyCon = jv.GetKeyHidden(p);
                    valCon = jv.GetValueDisplay(p);
                }
                else {
                    keyCon = jv.GetKeyHidden(p).val(val.id); // $(p.obj).find("input:hidden").val(val.id);
                    valCon = jv.SetDisplayValue(p, [val.id], [val.name], evt); // val.name);
                }

                if (p.callback) { p.callback(role, data, grid, { originalEvent: true, target: p.obj }); }

                //触发 Hidden 值的 Change 事件.
                if (p.data.code != val.id) {
                    keyCon.enforceTrigger("change");
                    valCon.enforceTrigger("change");
                }
            };
        }
        else if (p.mode == "check") {
            window._Ref_Callback_ = function (role, data, grid) {

                if (jv.IsNull(data)) data = [];
                if (jv.IsNull(role)) role = { id: "", name: "" };

                var keyCon, valCon;
                if (p.refClick &&
                   (p.refClick(role, data, grid, { originalEvent: true, target: p.obj }) === false)) {

                    keyCon = jv.GetKeyHidden(p);
                    valCon = jv.GetValueDisplay(p);
                }
                else {
                    var key = [], val = [];
                    $.each(data, function () {
                        var item = GetRefValue(this, role);
                        key.push(item["id"]);
                        val.push(item["name"]);
                    });
                    keyCon = jv.GetKeyHidden(p).val(key.join(","));
                    valCon = jv.SetDisplayValue(p, key, val, { originalEvent: true, target: grid.bDiv }); //val.join(","));
                }

                if (p.callback) { p.callback(role, data, grid, { originalEvent: true, target: p.obj }); }

                //触发 Hidden 值的 Change 事件.
                if (p.data.code != keyCon.val()) {
                    keyCon.enforceTrigger("change");
                    valCon.enforceTrigger("change");
                }
            };
        }
        else { window._Ref_Callback_ = p.refClick || p.callback; }

        var pq = [];

        if (p.data.code && (p.data.code != "0")) {
            var pqCodes = p.data.code.mySplit(",", true);
            var pqNames = p.data.name.mySplit(",", true);
            $.each(pqCodes, function (i, d) { pq.push({ id: d, name: pqNames[i] }); });
        }
        window._PageQuote_ = pq;



        var whConfig = jv.PopListConfig(p.area, p.entity, p.list);

        if (!p.width) {
            p.width = whConfig[0];
        }

        if (!p.height) {
            p.height = whConfig[1];
        }

        p.openMode = whConfig[2];

        if (!p.url) {
            p.url = jv.Root + (p.area + "/" || "") + p.entity + "/" + p.list;
            if (p.uid) {
                p.url += "/" + p.uid + ".aspx";
            }
            else {
                p.url += ".aspx"
            }
            p.url = jv.url(p.url);
        }

        if (p.query) {
            p.url.attr(p.query);
        }


        if (p.mode) {
            p.url.attr("_Ref_Type_", p.mode);
        }

        jv.Pop(p.url.tohref ? p.url.tohref() : p.url, p);
    };

    ////////////////////////////////////////////////////////////////////////////////////////////
    //改变 网页地址 . http://hachiko.sinaapp.com/?p=61
    jv.Goto = jv["goto"] = function (url) {
        if (jv.InModal()) {
            $.get(url, function (res) {
                document.write("");
                document.close();

                document.write(res);
            });
            return;
        }

        if ($.browser.msie) {
            var referLink = document.createElement('a');
            referLink.href = url;
            document.body.appendChild(referLink);
            referLink.click();
        } else {
            location.href = url;
        }
    };


    jv.GetInt = jv.getInt = function (Value, defValueFunc) {
        if (Value === "thin") return 1;
        else if (Value === "medium") return 2;
        else if (Value === "thick") return 3;

        var ret = parseInt(Value);
        if (ret === 0) return ret;

        return ret ||
        (defValueFunc
            ? $.isFunction(defValueFunc)
                ? defValueFunc(Value)
                : defValueFunc
            : 0
         )
        ;
    }

    jv.TimeSpan = jv.timeSpan = function (edate, sdate) {
        var getSpan = function (d) {
            return parseFloat(d.getHours() * 600 + d.getMinutes() * 60 + d.getSeconds() + "." + d.getMilliseconds());
        }
        return getSpan(edate) - getSpan(sdate);
    };


    jv.UnOneClick = jv.unOneClick = function (obj) {
        if (obj) $(obj).data("NoOneClick", true);

        $.timer(100, function (timer) {
            timer.stop();
            $($(document).data("OneClick") || []).removeAttr("disabled");
            $(document).data("OneClick", null);
            document.body.style.cursor = "default";
        });
    };

    //回退机制： 1.URL 判断 ReturnUrl 。2.采用服务器记录 History 机制。3.document.referrer 4. history.back
    jv.Back = jv.back = function () {
        var retUrl = jv.url().attr("ReturnUrl");
        if (retUrl) {
            var url = jv.url(decodeURIComponent(retUrl));
            url.attr("_", jv.random());
            if (url) {
                jv["goto"](url.toString());
                return;
            }
        }

        var his = $.parseJSON($.cookie("History"));
        if (his && his.length > 0) {

            //查找最近一个。 

            for (var i = his.length - 1; i >= 0; i--) {
                if (jv.page().href != his[i]) {
                    his = his.removeAt(i);
                    continue;
                }
                else break;
            }

            his = his.removeAt(his.length - 1);
            $.cookie("History", $.toJSON(his), { path: "/", expires: 1 });

            if (his.length > 0) {
                jv["goto"](his[his.length - 1]);
                return;
            }
        }

        if (document.referrer) {
            var url = jv.url(document.referrer);
            url.attr("_", jv.random());
            if (url) {
                jv["goto"](url.toString());
                return;
            }
        }

        history.back();
    };

    //仿 qq.toElement
    jv.toElement = jv.ToElement = (function () {
        var div = document.createElement('div');
        return function (html) {
            div.innerHTML = html;
            var element = div.firstChild;
            div.removeChild(element);
            return element;
        };
    })();

    //导出
    jv.PostOpen = jv.postOpen = function (url, postJson, op) {

        var entity = op.entity;
        var target = op.target || entity;

        var _createIframe = function (id) {
            var iframe = jv.toElement('<iframe src="javascript:false;" name="' + id + '" />');
            iframe.setAttribute('id', id);
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
            return iframe;
        },
        _createForm = function (target) {
            var form = jv.toElement('<form method="POST"></form>');
            form.setAttribute('action', url);
            form.setAttribute('target', target);
            form.style.display = 'none';

            var isSimple = function (type) {
                return ["string", "number", "date"].indexOf(type) >= 0;
            };


            var json = {};
            var rec = function (obj, prefix) {
                if (jv.IsNull(obj)) return;

                if (prefix) prefix += ".";

                for (var k in obj) {
                    var v = obj[k];
                    var t = jv.getType(v);
                    if (isSimple(t)) {
                        json[prefix + k] = v;
                        continue;
                    }
                    rec(v, prefix + k);
                }
            };


            rec(postJson, "");

            if (json) {
                for (var k in json) {
                    form.appendChild(jv.toElement('<input type="hidden" name="' + k + '" value="' + json[k] + '" />'));
                }
            }

            document.body.appendChild(form);
            return form;
        };


        if (op.self) {
            var iframe = _createIframe(target);

            var fn = function () {
                if (!iframe.parentNode) {
                    return;
                }


                var response;
                var doc = iframe.contentDocument ? iframe.contentDocument : iframe.contentWindow.document;

                try {
                    var innerHTML;
                    var $html = $(doc.body.innerHTML);
                    var $pre = $html.find("pre");
                    if ($pre.length == 0) innerHTML = $html.text();
                    else innerHTML = $pre.text();

                    //ie 返回的pre内容里，有 font ，还额外赠送一个 ^ 号。
                    if (innerHTML.slice(-1) == '^') innerHTML = innerHTML.slice(0, -1);

                    //                    if (innerHTML.length > 10 && innerHTML.slice(0, 4).toLowerCase() == '<pre' && innerHTML.slice(-6).toLowerCase() == '</pre>') {
                    //                        innerHTML = doc.body.firstChild.firstChild.nodeValue;
                    //                    }

                    if (innerHTML.length > 1 && innerHTML.slice(0, 1) != "<") {
                        if (innerHTML.slice(0, 1) == "{" && innerHTML.slice(-1) == "}") {
                            response = $.parseJSON(innerHTML);
                        }
                        else {
                            response = jv.execJs(innerHTML);
                        }
                    }
                } catch (err) {
                    response = { success: false };
                }

                if (response && response.extraJs) jv.execJs(response.extraJs);

                if (op.callback) op.callback(response, doc);
            };

            //优先使用 attachEvent , 解决IE9下的BUG。
            if (iframe.attachEvent) {
                iframe.attachEvent('onload', fn);
            }
            else if (iframe.addEventListener) {
                iframe.addEventListener('load', fn, false);
            }
        }
        else {
            var p = { entity: entity };
            var whConfig = jv.PopListConfig(p.area, p.entity, p.detail);

            if (!p.width) {
                p.width = whConfig[0];
            }

            if (!p.height) {
                p.height = whConfig[1];
            }

            p.openMode = whConfig[2] || target;

            //多次打印多个窗口

            p.entity = entity;
            p.autoFocus = false;

            jv.Pop("", p);
        }


        var form = _createForm(target);
        form.submit();
    };


    //==================================================================================
    //只执行一次, 去抖动. http://blog.csdn.net/phpandjava/article/details/5860355
    jv.Debounce = jv.debounce = function (func, threshold, execAsap) {
        var timeout;
        return function debounced() {
            var obj = this, args = arguments;
            function delayed() {
                if (!execAsap)
                    func.apply(obj, args);
                timeout = null;
            };
            if (timeout)
                clearTimeout(timeout);
            else if (execAsap)
                func.apply(obj, args);
            timeout = setTimeout(delayed, threshold || 100);
        };
    }


    jv.GetFillHeightValue = jv.getFillHeightValue = function (con) {
        var jd = $(con);
        var pjd = jd.parent(), h;

        if (jd[0].tagName == "BODY") {
            //规定：　HTML 下只有一个 BODY 且其 offsetTop , offsetLeft 均为0.
            // document.documentElement.clientHeight 是可见区域的大小 
            // document.documentElement.scrollHeight 是body内容的大小，包含：body 的　margin,border,padding

            //可见区域大，则使用　可见区域 - html.margin - html.border -html.padding - body.margin - body.border - body.padding
            try {
                h = window.opener && (window.opener.popArg.height - 1/*传进来的是900，但实际可见高度是 899，差一个像素。不知为何*/);
            }
            catch (e) { }

            h = h || document.documentElement.clientHeight;

            h = h - jv.GetInt(pjd.css("marginTop"))
                - jv.GetInt(pjd.css("marginBottom"))
                - jv.GetInt(pjd.css("borderTopWidth"))
                - jv.GetInt(pjd.css("borderBottomWidth"))
                - jv.GetInt(pjd.css("paddingTop"))
                - jv.GetInt(pjd.css("paddingBottom"))
                - jv.GetInt(jd.css("marginTop"))
                - jv.GetInt(jd.css("marginBottom"))

                - jv.GetInt(jd.css("borderTopWidth"))
                - jv.GetInt(jd.css("borderBottomWidth"))

                - jv.GetInt(jd.css("paddingTop"))
                - jv.GetInt(jd.css("paddingBottom"))
            ;
            //if (document.documentElement.clientHeight > document.documentElement.scrollHeight) {
            //}
            //else {
            //    //有滚动条，则
            //    //document.documentElement.scrollHeight =  body的border大小 -   body.border - body.padding

            //    h = document.documentElement.scrollHeight;

            //    h = h - jv.GetInt(jd.css("borderTopWidth"))
            //        - jv.GetInt(jd.css("borderBottomWidth"))

            //        - jv.GetInt(jd.css("paddingTop"))
            //        - jv.GetInt(jd.css("paddingBottom"))
            //    ;
            //}
        }
        else if (jd[0].tagName == "HTML") {
            throw Error("不合法");
        }
        else {
            //如果是 table  tbody thead tfoot tr ,则使用只能有一个使用 FillHeight = TableContainer.Height - Table.Height
            if ($.inArray(jd[0].tagName, ["TBODY", "THEAD", "TFOOT", "TR", "TD", "TH"]) >= 0) {
                if (pjd.siblings().length === 0) {
                    var ptab = pjd.closest("table");
                    h = ptab.parent().height() - ptab.height();
                }

                h = Math.max(parseInt(jv.oriCss(pjd[0], "minHeight") || 0), parseInt(pjd.oriCss("height") || 0));
            }
            else {
                h = pjd.height();
            }

            /*
            a 是容器， b 是自适应高度的对象
            h 是指 ContentHeight ，不包含 padding
            getBoundingClientRect 是border 的位置
            公式： 
            abJx = ab 间隙 
            abX = ab之间的getBoundingClientRect的差。 b.getBoundingClientRect - a.getBoundingClientRect
            abX = a.bt + a.pt + abJx + b.mt
            b.h = a.h - abJx - b.margin - b.border -b.padding
            => b.h = a.h  - ( abX - a.bt - a.pt - b.mt ) - b.margin - b.border -b.padding
            => b.h = a.h - abX + a.bt + a.pt + b.mt - b.margin - b.border -b.padding
            => b.h = a.h - abX + a.bt + a.pt - b.mb - b.border -b.padding
            */
            h = h
                - (jd[0].getBoundingClientRect().top - pjd[0].getBoundingClientRect().top)
                + jv.GetInt(pjd.css("borderTopWidth"))
                + jv.GetInt(pjd.css("paddingTop"))
                - jv.GetInt(jd.css("marginBottom"))
                - jv.GetInt(jd.css("borderBottomWidth"))
                - jv.GetInt(jd.css("borderTopWidth"))
                - jv.GetInt(jd.css("paddingTop"))
                - jv.GetInt(jd.css("paddingBottom"))
            ;
        }

        h += jv.GetInt(jd.attr("ofvh")); //再加上一个偏移量.  OffsetValue

        return h;
    };

    jv.fixHeight = jv.FixHeight = function (d) {
        //var selector = selector; //这句并不是废话, 不这样写的话. each 里面获取不到 selector 变量. 

        var jd = $(d), theClass;
        theClass = (jd.hasClass("FillHeight") ? "FillHeight" : "FillHeight-");

        if (d.tagName != "BODY") {
            var pjd = jd.parent(), ppjd;
            ppjd = pjd.closest(".FillHeight,.FillHeight-,BODY");
            if (ppjd.length > 0) {
                if (jv.fixHeight(ppjd[0]) <= 0) return 0;
            }
        }


        var min = jd.hasClass("FillMinHeight"), max = jd.hasClass("FillMaxHeight");

        var h = jv.GetFillHeightValue(jd);

        if (!jd.hasClass("FillToBottom") && !jd.hasClass("divSplit")) {
            /*
            再减去 该元素的后续元素的整体高度.
            前提： 第一个元素是后续元素中海拔最高的元素，最后一个元素是后续元素中海拔最低的元素
            outerHeight = offsetHeight = 包含 border 的高度

                如果后续的元素个数大于1个。
                var d1 = 第一个弟弟
                var last = 最后一个弟弟
                var th? = FillHeight元素的弟弟总高度
                th?  = last.上顶点.top - d1.的上顶点.top + last.offsetHeight + d1.marginTop + last.marginBottom 

                如果后续的元素个数 == 1个。
                var th? = d1.offsetHeight + d1.margin 
            */
            var findNode = function (n) {
                if (n.display == "none") return false;
                if (["SCRIPT", "STYLE", "META", "LINK"].indexOf(n.tagName) >= 0) return false;
                if (["absolute", "fixed"].indexOf(n.style.position) >= 0) return false;
                return true;
            }

            var next1 = jv.getNextNode(d, findNode);
            var next2 = jv.getNextNode(next1, findNode);

            //jv.getLastNode 如果 查找的元素 是最后一个，则返回 该元素。所以使用  next2
            var last1 = jv.getLastNode(next2, findNode);

            if (next1 && last1) {
                h -= (last1.getBoundingClientRect().top - next1.getBoundingClientRect().top + last1.offsetHeight + jv.getInt(jv.css(next1, "marginTop")) + jv.getInt(jv.css(next1, "marginBottom")));
            }
            else if (next1) {
                h -= (next1.offsetHeight + jv.getInt(jv.css(next1, "marginTop")) + jv.getInt(jv.css(next1, "marginBottom")));
            }
        }

        if (h > 0) {

            if (!min && !max) {
                jd.height(h + "px");
            }
            else if (min) {
                jd.css("minHeight", h + "px");
                if ($.browser.msie) {
                    jd.addClass("heightAuto").css("height", h + "px");
                }
            }
            else if (max) {
                jd.css("maxHeight", h + "px");
            }

            jd.addClass("FillHeighted").removeClass(theClass);
        }
        else {
            //debugger;
            //throw new Error("取FillHeight高度怎么是：" + h + " ？！");
        }
        return h;
    };

    jv.GetFillWidthValue = jv.getFillWidthValue = function (jd) {
        var pjd = jd.parent(), h;

        if (pjd[0].tagName == "BODY") {
            //火狐下有可能小于0.
            var documentWidth = Math.max($(document).width(), document.documentElement.scrollWidth); // document.documentElement.offsetHeight);

            h = documentWidth
            - jv.GetInt(pjd.css("marginLeft"))
            - jv.GetInt(pjd.css("marginRight"))
            - jv.GetInt(pjd.css("borderLeftWidth"))
            - jv.GetInt(pjd.css("borderRightWidth"))
            - jv.GetInt(pjd.css("paddingLeft"))
            - jv.GetInt(pjd.css("paddingRight"))
            ;
        }
        else {
            h = pjd.width();
        }

        h = h
        - (jd.offset().left - pjd.offset().left)
        + jv.GetInt(pjd.css("borderLeftWidth"))
        + jv.GetInt(pjd.css("paddingLeft"))
        - jv.GetInt(jd.css("borderLeftWidth"))
        - jv.GetInt(jd.css("borderRightWidth"))


        - jv.GetInt(jd.css("paddingLeft"))
        - jv.GetInt(jd.css("paddingRight"))

        - jv.GetInt(jd.css("marginRight"));


        h += jv.GetInt(jd.attr("ofvw")); //再加上一个偏移量.  OffsetValue

        return h;
    };

    jv._fixWidth = function (d) {
        //var selector = selector; //这句并不是废话, 不这样写的话. each 里面获取不到 selector 变量. 

        var jd = $(d), theClass = (jd.hasClass("FillWidth") ? "FillWidth" : "FillWidth-");

        var ppjd = jd.parents("." + theClass + ":first");
        if (ppjd.length > 0) jv._fixWidth(ppjd[0]);


        var h = jv.GetFillWidthValue(jd);

        if (!jd.hasClass("FillToRight")) {
            //减该元素的弟弟元素的高度.
            jd.nextAll()
            .filter(":visible")
            .filter(function () {
                var self = $(this), pos = self.css("position");
                if (pos == "absolute") return false;
                if (pos == "relative" && jv.oriCss(self[0], "left")) return false;
                return true;
            })
            .each(function () {
                h -= $(this).outerWidth(true);
            });
        }

        if ($.browser.msie) h--;

        if (h > 0) {
            jd.width(h).addClass("FilledWidth").removeClass(theClass);
        }
        return h;
    };




    jv.SetDisable = jv.setDisable = function (jThis, timeout) {
        jThis.attr("disabled", "disabled");


        if (jThis.data("NoOneClick")) {
            jThis.data("NoOneClick", false);
            if (jv.__OneCliekTimer__) jv.__OneCliekTimer__.stop();
            jThis.removeAttr("disabled");
        }


        var OneClickControls = $(document).data("OneClick") || [];
        OneClickControls.push(jThis[0]);
        $(document).data("OneClick", OneClickControls);


        if (jv.__OneCliekTimer__) jv.__OneCliekTimer__.stop();
        jv.__OneCliekTimer__ = $.timer(timeout || 9000, function (timer) {
            timer.stop();
            $($(document).data("OneClick") || []).removeAttr("disabled");
            $(document).data("OneClick", null);
        });
    };

    jv.MyLoadOn["PinWidth"] = function (container) {
        $(".PinWidth", (container || document)).each(function () {
            var jd = $(this);
            jd.width(jd.width() - jv.GetInt(jd.attr("ofvw")));
        });
    };


    //--------------------------------------------------------------------------------------
    //FillHeight-  仅在 OnLoad 时执行.
    jv.MyLoadOn["FillHeight-"] = function (container) {
        container = container || document;
        $(".FillHeight-", container).add(".FillHeight.divSplit", container).each(function () {
            jv.fixHeight(this);
        });
    };

    jv.MyLoadOn["FillWidth-"] = function (container) {
        $(".FillWidth-", (container || document)).each(function () {
            jv._fixWidth(this);
        });
    };

    jv.MyInitOn["FillHeight"] = function (container) {
        //var $body = $(document.body);
        //if (!$body.hasClass("FillHeight") && !$body.hasClass("FillHeight-")) {
        //    $body.addClass("FillHeight").addClass("FillMinHeight");
        //    $body.find(">form").addClass("FillHeight").addClass("FillMinHeight");
        //}


        $(".FillHeight", (container || document))
        //去除 divSplit
        .filter(function () { return !$(this).hasClass("divSplit"); })
        .each(function () {
            jv.fixHeight(this);
        });
    };

    jv.MyInitOn["FillWidth"] = function (container) {
        $(".FillWidth", (container || document)).each(function () {
            jv._fixWidth(this);
        });
    };




    jv.MyInitOn["MyTool"] = function (container) {
        container = container || document;
        //$(".MyCardTitle",container).MyCardTitle().MyCorner();

        //把 .MyTool 移到 .PageTool , 如果有的话.
        $(".MyTool,.myTool", container).each(function () {
            var myTool = $(this);
            if (myTool.hasClass("pin")) return;

            //如果有 .PageTool , 则附到 .PageTool 里,如果在 Boxy 里, 按钮附到 .BoxyTool 里.
            var pageTool = $(".PageTool", jv.boxdy(this));
            if (pageTool.length) {
                myTool.children().not(".view").prependTo(pageTool);
                myTool.hide();
                return;
            }
            else {
                var boxyInner = myTool.closest(".boxy-inner");
                if (boxyInner.length) {
                    myTool.children().prependTo(boxyInner.find(".BoxyTool"));
                    myTool.hide();
                    return;
                }
            }

            if (!jv.getNode(this).length) {
                myTool.hide();
                return;
            }
        });

    };

    // note by haojun 商铺暂且不需要 回车触发提交功能 2013-5-23 
    //jv.MyLoadOn["Card"] = function (container) {
    //    var container = container || jv.boxdy();
    //    //$(".MyCardTitle",container).MyCardTitle().MyCorner();

    //    //避免多次执行事件绑定.
    //    if (container.data("jv.CardExeced")) return true;
    //    container.data("jv.CardExeced", true);


    //    //强制重排
    //    container.find(".MyCard").css("position", "relative");

    //    var btn = container.find("button.submit");
    //    var inp = container.find("input[type=button].submit");

    //    //WebForm 的服务器控件<button> 会生成 <input type="submit" />，为了不影响它，对这类跳过一次点击。 by udi@2012年12月11日
    //    //        container
    //    //        .find("button[type=submit]")
    //    inp
    //    .add(btn)
    //    .OneClick();


    //    if (container.IsBindEvent("keyup") || container.IsBindEvent("keydown") || container.IsBindEvent("keypress")) {
    //    }
    //    else {
    //        container.bind("keyup.enter", function (ev) {

    //            //这两个变量不能放在外面进行缓存，因为：当该页面是LoadView 进来的时候，可能Dom已消失。再用缓存的Dom进行查找，会找不到相应的 jv.page
    //            var btn = container.find("button.submit");
    //            var inp = container.find("input[type=button].submit");

    //            var enter;
    //            if ((inp.length > 0) && (btn.length > 0)) {
    //                enter = container.find("button.submit.default").add(container.find("input[type=button].submit.default"))
    //            }
    //            else if (btn.length) {
    //                enter = btn.first();
    //            }
    //            else if (inp.length) {
    //                enter = inp.first();
    //            }

    //            if (enter && (ev.keyCode == 13)) {
    //                var doer = jv.GetDoer();
    //                if (doer && (doer.tagName.toLowerCase() == "textarea")) return;

    //                if (enter[0].disabled) return;
    //                enter.trigger("click");
    //            }
    //        });
    //    }
    //};

    jv.LoadingDiv = jv.loadingDiv = function () {
        var $body = $(document.body);
        $body.find(">div.loadingDiv").remove();

        $body.coverDiv();

        var size = jv.getEyeSize();
        var cover = document.createElement("div");
        var $cover = $(cover);
        $cover.addClass("loadingDiv");
        cover.style.top = "-3000px";

        $body.prepend(cover);

        cover.style.top = parseInt((size.height - parseInt($cover.outerHeight())) / 2) + "px";
        cover.style.left = parseInt((size.width - parseInt($cover.outerWidth())) / 2) + "px";

        return {};
    };




    jv.BindRefClick = jv.bindRefClick = function (obj) {

        var jd = $(obj);
        if (jd.data("refed")) return true;
        jd.data("refed", "true");

        jd.data("oriclick", obj.onclick);

        var joclick = [];
        if (jd.data("events") && jd.data("events")["click"]) {
            var clicks = jd.data("events")["click"];
            for (var i = 0; i < clicks.length; i++) {
                joclick.push(clicks[i]);
            }
        }
        jd.data("orijclick", joclick);

        jd.unbind("click");
        obj.onclick = null;

        jd.click(function (ev) {
            var ret;
            var jobj = $(jv.GetDoer());
            if (ev.isTrigger || (jobj.offset().left + jobj.width()
                /*+ jv.GetInt( jobj.css("paddingLeft")) + jv.GetInt( jobj.css("paddingRight")) */
                - 24 < ev.clientX)) {

                var keyClick = jobj.data("oriclick"),
                    jclick = jobj.data("orijclick");

                if (keyClick) ret = keyClick(ev);
                if (ret === false) return false;
                if (jclick && jclick.length) {
                    for (var i = 0; i < jclick.length; i++) {
                        if (jclick[i].handler) ret = jclick[i].handler(ev);
                        if (ret === false) return false;
                    }
                }

            }

            return ret;
        });
    };




    //jv.MyInitOn["jv.chk.url"] = jv.MyLoadOn["jv.chk.url"] = function (alwaysLoad, callback) {
    //    jv.chk = jv.chk || function () { return true; };

    //    if (!jv.chk && (alwaysLoad || $("[chk]:first").length)) {
    //        jv.LoadJsCss("css", jv.Root + "Res/poshytip/tip-yellow/tip-yellow.css");
    //        jv.LoadJsCss("js", jv.Root + "Res/MyJs_chk.js", callback);
    //    }
    //};


    //在文档加载完Js后，最初执行。
    jv.MyOnInit = jv.myOnInit = function (container) {
        for (var key in jv.MyInitOn) {
            if (!key) continue;

            jv.MyInitOn[key](container);
        }
    };


    //在文档加载完Js后，最末执行。
    jv.MyOnLoad = jv.myOnLoad = function (container) {
        for (var key in jv.MyLoadOn) {
            if (!key) continue;

            jv.MyLoadOn[key](container);
        }

        /*
        <td class="key">
        业主名称：<input type="hidden" name="List_CustName" />
        </td>
        <td class="val">
        <input readonly="readonly" class="ref"  onclick="alert(1);"/>
        </td>
        */
        $("input.ref", (container || document)).each(function () {
            jv.BindRefClick(this);
        });
    };


    jv.Clone = jv.clone = function clone(obj) {
        var objClone;
        if (obj.constructor == Object) {
            objClone = new obj.constructor();
        } else {
            objClone = new obj.constructor(obj.valueOf());
        }
        for (var key in obj) {
            if (objClone[key] != obj[key]) {
                if (typeof (obj[key]) == "object") {
                    objClone[key] = clone(obj[key]);
                } else {
                    objClone[key] = obj[key];
                }
            }
        }
        objClone.toString = obj.toString;
        objClone.valueOf = obj.valueOf;
        return objClone;
    };

    jv.Dom = jv.dom = function (tag) {
        this.tag = tag;
        this.setAttribute = function (atrName, val) { this[atrName] = val; };
        this.style = {
            toString: function () {
                var keys = jv.GetJsonKeys(this);
                var ary = [];
                for (var i = 0, len = keys.length; i < len; i++) {
                    var key = keys[i];
                    if (key == "toString") continue;
                    var val = this[key];
                    if (jv.IsNull(val)) continue;
                    ary.push(key + ":" + val);
                }

                return ary.join(";");
            }
        };
        this.innerHTML = "";
        this.children = [];
        this.clone = function () {
            return jv.clone(this);
        };
        this.toString = function () {
            var keys = jv.GetJsonKeys(this);
            var ary = [];
            for (var i = 0, len = keys.length; i < len; i++) {
                var key = keys[i];
                if (key == "tag") continue;
                if (key == "setAttribute") continue;
                if (key == "toString") continue;
                if (key == "innerHTML") continue;
                if (key == "children") continue;
                if (key == "clone") continue;
                var val = this[key];
                if (jv.IsNull(val)) continue;
                ary.push(key + '="' + val + '"');
            }

            return "<" + this.tag + " " + ary.join(" ") + " >" +
                (this.innerHTML ? this.innerHTML : (function (con) {
                    var ary = [];
                    for (var i = 0, len = con.children.length ; i < len ; i++) {
                        ary.push(con.children[i].toString());
                    }
                    return ary.join("");
                })(this)) +
                "</" + this.tag + ">";
        };
        return this;
    };


    jv.TypeIsNumberType = jv.typeIsNumberType = function (typeString) {
        return ["number", "int", "Int32", "UInt32", "Int16", "UInt16", "Single", "Byte", "SByte", "Int64", "Decimal", "Double", "UInt64", "VarNumeric", "Currency", "Decimal"].indexOf(typeString) >= 0;
    };

    //修复IE7双向滚动条的问题。
    //http://remysharp.com/demo/overflow.html
    jv.FixIe7 = jv.fixIe7 = function (dom) {
        if ($.browser.msie && (parseInt($.browser.version) === 7)) {
            if (dom.scrollHeight > dom.offsetHeight) {
                dom.style['overflowY'] = 'auto';
                dom.style['paddingBottom'] = '0';
            }
            else if (dom.scrollWidth > dom.offsetWidth) {
                dom.style['overflowY'] = 'hidden';
                dom.style['paddingBottom'] = '20px';
            }
        }
    };

    //window.onresize = jv.debounce(function () {
    //    $(".FillHeighted")
    //    .height("30px")
    //    .addClass("FillHeight")
    //    .removeClass("FillHeighted")
    //    .each(function () {
    //        var self = $(this);
    //        if (self.hasClass("FillMinHeight")) { self.css("minHeight", "auto"); }
    //    });

    //    if (jv.MyLoadOn["FillHeight"]) {
    //        jv.MyLoadOn["FillHeight"]();
    //        return;
    //    }
    //}, 100, true);


    /*
        jv.RestartTimer("TextHelper",function(){ if ( this.die ) return false;})
    */
    jv.RestartTimer = function (key,func) {
        if (!window.restart_timer_dict) {
            window.restart_timer_dict = {};
        }
        
        if (window.restart_timer_dict[key])
        {
            window.restart_timer_dict[key].die = true;
            delete window.restart_timer_dict[key];
        }

        window.restart_timer_dict[key] = func;

        $.timer(500, function (timer) {
            timer.stop();
            window.restart_timer_dict[key]();
        });
    }


    $(function () {
        $(document.body).ajaxSend(function (e, xhr, opt) {
            if (opt.type == "POST") {
                xhr.setRequestHeader("User_Target_Title", document.title);
                var doer = jv.getDoer();
                if (doer) {
                    xhr.setRequestHeader("User_Target_Element", doer.id || doer.name || doer.value || doer.tagName);
                }
            }
        });
    });

})();