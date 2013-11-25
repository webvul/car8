/*!
* jQuery JavaScript Library v1.7.1
* http://jquery.com/
*
* Copyright 2011, John Resig
* Dual licensed under the MIT or GPL Version 2 licenses.
* http://jquery.org/license
*
* Includes Sizzle.js
* http://sizzlejs.com/
* Copyright 2011, The Dojo Foundation
* Released under the MIT, BSD, and GPL Licenses.
*
* Date: Mon Nov 21 21:11:03 2011 -0500
*/
(function (window, undefined) {

    // Use the correct document accordingly with window argument (sandbox)
    var document = window.document,
	navigator = window.navigator,
	location = window.location;
    var jQuery = (function () {

        // Define a local copy of jQuery
        var jQuery = function (selector, context) {
            // The jQuery object is actually just the init constructor 'enhanced'
            return new jQuery.fn.init(selector, context, rootjQuery);
        },

        // Map over jQuery in case of overwrite
	_jQuery = window.jQuery,

        // Map over the $ in case of overwrite
	_$ = window.$,

        // A central reference to the root jQuery(document)
	rootjQuery,

        // A simple way to check for HTML strings or ID strings
        // Prioritize #id over <tag> to avoid XSS via location.hash (#9521)
	quickExpr = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/,

        // Check if a string has a non-whitespace character in it
	rnotwhite = /\S/,

        // Used for trimming whitespace
	trimLeft = /^\s+/,
	trimRight = /\s+$/,

        // Match a standalone tag
	rsingleTag = /^<(\w+)\s*\/?>(?:<\/\1>)?$/,

        // JSON RegExp
	rvalidchars = /^[\],:{}\s]*$/,
	rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,
	rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
	rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g,

        // Useragent RegExp
	rwebkit = /(webkit)[ \/]([\w.]+)/,
	ropera = /(opera)(?:.*version)?[ \/]([\w.]+)/,
	rmsie = /(msie) ([\w.]+)/,
	rmozilla = /(mozilla)(?:.*? rv:([\w.]+))?/,

        // Matches dashed string for camelizing
	rdashAlpha = /-([a-z]|[0-9])/ig,
	rmsPrefix = /^-ms-/,

        // Used by jQuery.camelCase as callback to replace()
	fcamelCase = function (all, letter) {
	    return (letter + "").toUpperCase();
	},

        // Keep a UserAgent string for use with jQuery.browser
	userAgent = navigator.userAgent,

        // For matching the engine and version of the browser
	browserMatch,

        // The deferred used on DOM ready
	readyList,

        // The ready event handler
	DOMContentLoaded,

        // Save a reference to some core methods
	toString = Object.prototype.toString,
	hasOwn = Object.prototype.hasOwnProperty,
	push = Array.prototype.push,
	slice = Array.prototype.slice,
	trim = String.prototype.trim,
	indexOf = Array.prototype.indexOf,

        // [[Class]] -> type pairs
	class2type = {};

        jQuery.fn = jQuery.prototype = {
            constructor: jQuery,
            init: function (selector, context, rootjQuery) {
                var match, elem, ret, doc;

                // Handle $(""), $(null), or $(undefined)
                if (!selector) {
                    return this;
                }

                // Handle $(DOMElement)
                if (selector.nodeType) {
                    this.context = this[0] = selector;
                    this.length = 1;
                    return this;
                }

                // The body element only exists once, optimize finding it
                if (selector === "body" && !context && document.body) {
                    this.context = document;
                    this[0] = document.body;
                    this.selector = selector;
                    this.length = 1;
                    return this;
                }

                // Handle HTML strings
                if (typeof selector === "string") {
                    // Are we dealing with HTML string or an ID?
                    if (selector.charAt(0) === "<" && selector.charAt(selector.length - 1) === ">" && selector.length >= 3) {
                        // Assume that strings that start and end with <> are HTML and skip the regex check
                        match = [null, selector, null];

                    } else {
                        match = quickExpr.exec(selector);
                    }

                    // Verify a match, and that no context was specified for #id
                    if (match && (match[1] || !context)) {

                        // HANDLE: $(html) -> $(array)
                        if (match[1]) {
                            context = context instanceof jQuery ? context[0] : context;
                            doc = (context ? context.ownerDocument || context : document);

                            // If a single string is passed in and it's a single tag
                            // just do a createElement and skip the rest
                            ret = rsingleTag.exec(selector);

                            if (ret) {
                                if (jQuery.isPlainObject(context)) {
                                    selector = [document.createElement(ret[1])];
                                    jQuery.fn.attr.call(selector, context, true);

                                } else {
                                    selector = [doc.createElement(ret[1])];
                                }

                            } else {
                                ret = jQuery.buildFragment([match[1]], [doc]);
                                selector = (ret.cacheable ? jQuery.clone(ret.fragment) : ret.fragment).childNodes;
                            }

                            return jQuery.merge(this, selector);

                            // HANDLE: $("#id")
                        } else {
                            elem = document.getElementById(match[2]);

                            // Check parentNode to catch when Blackberry 4.6 returns
                            // nodes that are no longer in the document #6963
                            if (elem && elem.parentNode) {
                                // Handle the case where IE and Opera return items
                                // by name instead of ID
                                if (elem.id !== match[2]) {
                                    return rootjQuery.find(selector);
                                }

                                // Otherwise, we inject the element directly into the jQuery object
                                this.length = 1;
                                this[0] = elem;
                            }

                            this.context = document;
                            this.selector = selector;
                            return this;
                        }

                        // HANDLE: $(expr, $(...))
                    } else if (!context || context.jquery) {
                        return (context || rootjQuery).find(selector);

                        // HANDLE: $(expr, context)
                        // (which is just equivalent to: $(context).find(expr)
                    } else {
                        return this.constructor(context).find(selector);
                    }

                    // HANDLE: $(function)
                    // Shortcut for document ready
                } else if (jQuery.isFunction(selector)) {
                    return rootjQuery.ready(selector);
                }

                if (selector.selector !== undefined) {
                    this.selector = selector.selector;
                    this.context = selector.context;
                }

                return jQuery.makeArray(selector, this);
            },

            // Start with an empty selector
            selector: "",

            // The current version of jQuery being used
            jquery: "1.7.1",

            // The default length of a jQuery object is 0
            length: 0,

            // The number of elements contained in the matched element set
            size: function () {
                return this.length;
            },

            toArray: function () {
                return slice.call(this, 0);
            },

            // Get the Nth element in the matched element set OR
            // Get the whole matched element set as a clean array
            get: function (num) {
                return num == null ?

                // Return a 'clean' array
			this.toArray() :

                // Return just the object
			(num < 0 ? this[this.length + num] : this[num]);
            },

            // Take an array of elements and push it onto the stack
            // (returning the new matched element set)
            pushStack: function (elems, name, selector) {
                // Build a new jQuery matched element set
                var ret = this.constructor();

                if (jQuery.isArray(elems)) {
                    push.apply(ret, elems);

                } else {
                    jQuery.merge(ret, elems);
                }

                // Add the old object onto the stack (as a reference)
                ret.prevObject = this;

                ret.context = this.context;

                if (name === "find") {
                    ret.selector = this.selector + (this.selector ? " " : "") + selector;
                } else if (name) {
                    ret.selector = this.selector + "." + name + "(" + selector + ")";
                }

                // Return the newly-formed element set
                return ret;
            },

            // Execute a callback for every element in the matched set.
            // (You can seed the arguments with an array of args, but this is
            // only used internally.)
            each: function (callback, args) {
                return jQuery.each(this, callback, args);
            },

            ready: function (fn) {
                // Attach the listeners
                jQuery.bindReady();

                // Add the callback
                readyList.add(fn);

                return this;
            },

            eq: function (i) {
                i = +i;
                return i === -1 ?
			this.slice(i) :
			this.slice(i, i + 1);
            },

            first: function () {
                return this.eq(0);
            },

            last: function () {
                return this.eq(-1);
            },

            slice: function () {
                return this.pushStack(slice.apply(this, arguments),
			"slice", slice.call(arguments).join(","));
            },

            map: function (callback) {
                return this.pushStack(jQuery.map(this, function (elem, i) {
                    return callback.call(elem, i, elem);
                }));
            },

            end: function () {
                return this.prevObject || this.constructor(null);
            },

            // For internal use only.
            // Behaves like an Array's method, not like a jQuery method.
            push: push,
            sort: [].sort,
            splice: [].splice
        };

        // Give the init function the jQuery prototype for later instantiation
        jQuery.fn.init.prototype = jQuery.fn;

        jQuery.extend = jQuery.fn.extend = function () {
            var options, name, src, copy, copyIsArray, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

            // Handle a deep copy situation
            if (typeof target === "boolean") {
                deep = target;
                target = arguments[1] || {};
                // skip the boolean and the target
                i = 2;
            }

            // Handle case when target is a string or something (possible in deep copy)
            if (typeof target !== "object" && !jQuery.isFunction(target)) {
                target = {};
            }

            // extend jQuery itself if only one argument is passed
            if (length === i) {
                target = this;
                --i;
            }

            for (; i < length; i++) {
                // Only deal with non-null/undefined values
                if ((options = arguments[i]) != null) {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];

                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }

                        // Recurse if we're merging plain objects or arrays
                        if (deep && copy && (jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && jQuery.isArray(src) ? src : [];

                            } else {
                                clone = src && jQuery.isPlainObject(src) ? src : {};
                            }

                            // Never move original objects, clone them
                            target[name] = jQuery.extend(deep, clone, copy);

                            // Don't bring in undefined values
                        } else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }

            // Return the modified object
            return target;
        };

        jQuery.extend({
            noConflict: function (deep) {
                if (window.$ === jQuery) {
                    window.$ = _$;
                }

                if (deep && window.jQuery === jQuery) {
                    window.jQuery = _jQuery;
                }

                return jQuery;
            },

            // Is the DOM ready to be used? Set to true once it occurs.
            isReady: false,

            // A counter to track how many items to wait for before
            // the ready event fires. See #6781
            readyWait: 1,

            // Hold (or release) the ready event
            holdReady: function (hold) {
                if (hold) {
                    jQuery.readyWait++;
                } else {
                    jQuery.ready(true);
                }
            },

            // Handle when the DOM is ready
            ready: function (wait) {
                // Either a released hold or an DOMready/load event and not yet ready
                if ((wait === true && ! --jQuery.readyWait) || (wait !== true && !jQuery.isReady)) {
                    // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
                    if (!document.body) {
                        return setTimeout(jQuery.ready, 1);
                    }

                    // Remember that the DOM is ready
                    jQuery.isReady = true;

                    // If a normal DOM Ready event fired, decrement, and wait if need be
                    if (wait !== true && --jQuery.readyWait > 0) {
                        return;
                    }

                    // If there are functions bound, to execute
                    readyList.fireWith(document, [jQuery]);

                    // Trigger any bound ready events
                    if (jQuery.fn.trigger) {
                        jQuery(document).trigger("ready").off("ready");
                    }
                }
            },

            bindReady: function () {
                if (readyList) {
                    return;
                }

                readyList = jQuery.Callbacks("once memory");

                // Catch cases where $(document).ready() is called after the
                // browser event has already occurred.
                if (document.readyState === "complete") {
                    // Handle it asynchronously to allow scripts the opportunity to delay ready
                    return setTimeout(jQuery.ready, 1);
                }

                // Mozilla, Opera and webkit nightlies currently support this event
                if (document.addEventListener) {
                    // Use the handy event callback
                    document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);

                    // A fallback to window.onload, that will always work
                    window.addEventListener("load", jQuery.ready, false);

                    // If IE event model is used
                } else if (document.attachEvent) {
                    // ensure firing before onload,
                    // maybe late but safe also for iframes
                    document.attachEvent("onreadystatechange", DOMContentLoaded);

                    // A fallback to window.onload, that will always work
                    window.attachEvent("onload", jQuery.ready);

                    // If IE and not a frame
                    // continually check to see if the document is ready
                    var toplevel = false;

                    try {
                        toplevel = window.frameElement == null;
                    } catch (e) { }

                    if (document.documentElement.doScroll && toplevel) {
                        doScrollCheck();
                    }
                }
            },

            // See test/unit/core.js for details concerning isFunction.
            // Since version 1.3, DOM methods and functions like alert
            // aren't supported. They return false on IE (#2968).
            isFunction: function (obj) {
                return jQuery.type(obj) === "function";
            },

            isArray: Array.isArray || function (obj) {
                return jQuery.type(obj) === "array";
            },

            // A crude way of determining if an object is a window
            isWindow: function (obj) {
                return obj && typeof obj === "object" && "setInterval" in obj;
            },

            isNumeric: function (obj) {
                return !isNaN(parseFloat(obj)) && isFinite(obj);
            },

            type: function (obj) {
                return obj == null ?
			String(obj) :
			class2type[toString.call(obj)] || "object";
            },

            isPlainObject: function (obj) {
                // Must be an Object.
                // Because of IE, we also have to check the presence of the constructor property.
                // Make sure that DOM nodes and window objects don't pass through, as well
                if (!obj || jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow(obj)) {
                    return false;
                }

                try {
                    // Not own constructor property must be Object
                    if (obj.constructor &&
				!hasOwn.call(obj, "constructor") &&
				!hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                        return false;
                    }
                } catch (e) {
                    // IE8,9 Will throw exceptions on certain host objects #9897
                    return false;
                }

                // Own properties are enumerated firstly, so to speed up,
                // if last one is own, then all properties are own.

                var key;
                for (key in obj) { }

                return key === undefined || hasOwn.call(obj, key);
            },

            isEmptyObject: function (obj) {
                for (var name in obj) {
                    return false;
                }
                return true;
            },

            error: function (msg) {
                throw new Error(msg);
            },

            parseJSON: function (data) {
                if (typeof data !== "string" || !data) {
                    return null;
                }

                // Make sure leading/trailing whitespace is removed (IE can't handle it)
                data = jQuery.trim(data);

                // Attempt to parse using the native JSON parser first
                if (window.JSON && window.JSON.parse) {
                    return window.JSON.parse(data);
                }

                // Make sure the incoming data is actual JSON
                // Logic borrowed from http://json.org/json2.js
                if (rvalidchars.test(data.replace(rvalidescape, "@")
			.replace(rvalidtokens, "]")
			.replace(rvalidbraces, ""))) {

                    return (new Function("return " + data))();

                }
                jQuery.error("Invalid JSON: " + data);
            },

            // Cross-browser xml parsing
            parseXML: function (data) {
                var xml, tmp;
                try {
                    if (window.DOMParser) { // Standard
                        tmp = new DOMParser();
                        xml = tmp.parseFromString(data, "text/xml");
                    } else { // IE
                        xml = new ActiveXObject("Microsoft.XMLDOM");
                        xml.async = "false";
                        xml.loadXML(data);
                    }
                } catch (e) {
                    xml = undefined;
                }
                if (!xml || !xml.documentElement || xml.getElementsByTagName("parsererror").length) {
                    jQuery.error("Invalid XML: " + data);
                }
                return xml;
            },

            noop: function () { },

            // Evaluates a script in a global context
            // Workarounds based on findings by Jim Driscoll
            // http://weblogs.java.net/blog/driscoll/archive/2009/09/08/eval-javascript-global-context
            globalEval: function (data) {
                if (data && rnotwhite.test(data)) {
                    // We use execScript on Internet Explorer
                    // We use an anonymous function so that context is window
                    // rather than jQuery in Firefox
                    (window.execScript || function (data) {
                        window["eval"].call(window, data);
                    })(data);
                }
            },

            // Convert dashed to camelCase; used by the css and data modules
            // Microsoft forgot to hump their vendor prefix (#9572)
            camelCase: function (string) {
                return string.replace(rmsPrefix, "ms-").replace(rdashAlpha, fcamelCase);
            },

            nodeName: function (elem, name) {
                return elem.nodeName && elem.nodeName.toUpperCase() === name.toUpperCase();
            },

            // args is for internal usage only
            each: function (object, callback, args) {
                var name, i = 0,
			length = object.length,
			isObj = length === undefined || jQuery.isFunction(object);

                if (args) {
                    if (isObj) {
                        for (name in object) {
                            if (callback.apply(object[name], args) === false) {
                                break;
                            }
                        }
                    } else {
                        for (; i < length; ) {
                            if (callback.apply(object[i++], args) === false) {
                                break;
                            }
                        }
                    }

                    // A special, fast, case for the most common use of each
                } else {
                    if (isObj) {
                        for (name in object) {
                            if (callback.call(object[name], name, object[name]) === false) {
                                break;
                            }
                        }
                    } else {
                        for (; i < length; ) {
                            if (callback.call(object[i], i, object[i++]) === false) {
                                break;
                            }
                        }
                    }
                }

                return object;
            },

            // Use native String.trim function wherever possible
            trim: trim ?
		function (text) {
		    return text == null ?
				"" :
				trim.call(text);
		} :

            // Otherwise use our own trimming functionality
		function (text) {
		    return text == null ?
				"" :
				text.toString().replace(trimLeft, "").replace(trimRight, "");
		},

            // results is for internal usage only
            makeArray: function (array, results) {
                var ret = results || [];

                if (array != null) {
                    // The window, strings (and functions) also have 'length'
                    // Tweaked logic slightly to handle Blackberry 4.7 RegExp issues #6930
                    var type = jQuery.type(array);

                    if (array.length == null || type === "string" || type === "function" || type === "regexp" || jQuery.isWindow(array)) {
                        push.call(ret, array);
                    } else {
                        jQuery.merge(ret, array);
                    }
                }

                return ret;
            },

            inArray: function (elem, array, i) {
                var len;

                if (array) {
                    if (indexOf) {
                        return indexOf.call(array, elem, i);
                    }

                    len = array.length;
                    i = i ? i < 0 ? Math.max(0, len + i) : i : 0;

                    for (; i < len; i++) {
                        // Skip accessing in sparse arrays
                        if (i in array && array[i] === elem) {
                            return i;
                        }
                    }
                }

                return -1;
            },

            merge: function (first, second) {
                var i = first.length,
			j = 0;

                if (typeof second.length === "number") {
                    for (var l = second.length; j < l; j++) {
                        first[i++] = second[j];
                    }

                } else {
                    while (second[j] !== undefined) {
                        first[i++] = second[j++];
                    }
                }

                first.length = i;

                return first;
            },

            grep: function (elems, callback, inv) {
                var ret = [], retVal;
                inv = !!inv;

                // Go through the array, only saving the items
                // that pass the validator function
                for (var i = 0, length = elems.length; i < length; i++) {
                    retVal = !!callback(elems[i], i);
                    if (inv !== retVal) {
                        ret.push(elems[i]);
                    }
                }

                return ret;
            },

            // arg is for internal usage only
            map: function (elems, callback, arg) {
                var value, key, ret = [],
			i = 0,
			length = elems.length,
                // jquery objects are treated as arrays
			isArray = elems instanceof jQuery || length !== undefined && typeof length === "number" && ((length > 0 && elems[0] && elems[length - 1]) || length === 0 || jQuery.isArray(elems));

                // Go through the array, translating each of the items to their
                if (isArray) {
                    for (; i < length; i++) {
                        value = callback(elems[i], i, arg);

                        if (value != null) {
                            ret[ret.length] = value;
                        }
                    }

                    // Go through every key on the object,
                } else {
                    for (key in elems) {
                        value = callback(elems[key], key, arg);

                        if (value != null) {
                            ret[ret.length] = value;
                        }
                    }
                }

                // Flatten any nested arrays
                return ret.concat.apply([], ret);
            },

            // A global GUID counter for objects
            guid: 1,

            // Bind a function to a context, optionally partially applying any
            // arguments.
            proxy: function (fn, context) {
                if (typeof context === "string") {
                    var tmp = fn[context];
                    context = fn;
                    fn = tmp;
                }

                // Quick check to determine if target is callable, in the spec
                // this throws a TypeError, but we will just return undefined.
                if (!jQuery.isFunction(fn)) {
                    return undefined;
                }

                // Simulated bind
                var args = slice.call(arguments, 2),
			proxy = function () {
			    return fn.apply(context, args.concat(slice.call(arguments)));
			};

                // Set the guid of unique handler to the same of original handler, so it can be removed
                proxy.guid = fn.guid = fn.guid || proxy.guid || jQuery.guid++;

                return proxy;
            },

            // Mutifunctional method to get and set values to a collection
            // The value/s can optionally be executed if it's a function
            access: function (elems, key, value, exec, fn, pass) {
                var length = elems.length;

                // Setting many attributes
                if (typeof key === "object") {
                    for (var k in key) {
                        jQuery.access(elems, k, key[k], exec, fn, value);
                    }
                    return elems;
                }

                // Setting one attribute
                if (value !== undefined) {
                    // Optionally, function values get executed if exec is true
                    exec = !pass && exec && jQuery.isFunction(value);

                    for (var i = 0; i < length; i++) {
                        fn(elems[i], key, exec ? value.call(elems[i], i, fn(elems[i], key)) : value, pass);
                    }

                    return elems;
                }

                // Getting an attribute
                return length ? fn(elems[0], key) : undefined;
            },

            now: function () {
                return (new Date()).getTime();
            },

            // Use of jQuery.browser is frowned upon.
            // More details: http://docs.jquery.com/Utilities/jQuery.browser
            uaMatch: function (ua) {
                ua = ua.toLowerCase();

                var match = rwebkit.exec(ua) ||
			ropera.exec(ua) ||
			rmsie.exec(ua) ||
			ua.indexOf("compatible") < 0 && rmozilla.exec(ua) ||
			[];

                return { browser: match[1] || "", version: match[2] || "0" };
            },

            sub: function () {
                function jQuerySub(selector, context) {
                    return new jQuerySub.fn.init(selector, context);
                }
                jQuery.extend(true, jQuerySub, this);
                jQuerySub.superclass = this;
                jQuerySub.fn = jQuerySub.prototype = this();
                jQuerySub.fn.constructor = jQuerySub;
                jQuerySub.sub = this.sub;
                jQuerySub.fn.init = function init(selector, context) {
                    if (context && context instanceof jQuery && !(context instanceof jQuerySub)) {
                        context = jQuerySub(context);
                    }

                    return jQuery.fn.init.call(this, selector, context, rootjQuerySub);
                };
                jQuerySub.fn.init.prototype = jQuerySub.fn;
                var rootjQuerySub = jQuerySub(document);
                return jQuerySub;
            },

            browser: {}
        });

        // Populate the class2type map
        jQuery.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (i, name) {
            class2type["[object " + name + "]"] = name.toLowerCase();
        });

        browserMatch = jQuery.uaMatch(userAgent);
        if (browserMatch.browser) {
            jQuery.browser[browserMatch.browser] = true;
            jQuery.browser.version = browserMatch.version;
        }

        // Deprecated, use jQuery.browser.webkit instead
        if (jQuery.browser.webkit) {
            jQuery.browser.safari = true;
        }

        // IE doesn't match non-breaking spaces with \s
        if (rnotwhite.test("\xA0")) {
            trimLeft = /^[\s\xA0]+/;
            trimRight = /[\s\xA0]+$/;
        }

        // All jQuery objects should point back to these
        rootjQuery = jQuery(document);

        // Cleanup functions for the document ready method
        if (document.addEventListener) {
            DOMContentLoaded = function () {
                document.removeEventListener("DOMContentLoaded", DOMContentLoaded, false);
                jQuery.ready();
            };

        } else if (document.attachEvent) {
            DOMContentLoaded = function () {
                // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
                if (document.readyState === "complete") {
                    document.detachEvent("onreadystatechange", DOMContentLoaded);
                    jQuery.ready();
                }
            };
        }

        // The DOM ready check for Internet Explorer
        function doScrollCheck() {
            if (jQuery.isReady) {
                return;
            }

            try {
                // If IE is used, use the trick by Diego Perini
                // http://javascript.nwbox.com/IEContentLoaded/
                document.documentElement.doScroll("left");
            } catch (e) {
                setTimeout(doScrollCheck, 1);
                return;
            }

            // and execute any waiting functions
            jQuery.ready();
        }

        return jQuery;

    })();


    // String to Object flags format cache
    var flagsCache = {};

    // Convert String-formatted flags into Object-formatted ones and store in cache
    function createFlags(flags) {
        var object = flagsCache[flags] = {},
		i, length;
        flags = flags.split(/\s+/);
        for (i = 0, length = flags.length; i < length; i++) {
            object[flags[i]] = true;
        }
        return object;
    }

    /*
    * Create a callback list using the following parameters:
    *
    *	flags:	an optional list of space-separated flags that will change how
    *			the callback list behaves
    *
    * By default a callback list will act like an event callback list and can be
    * "fired" multiple times.
    *
    * Possible flags:
    *
    *	once:			will ensure the callback list can only be fired once (like a Deferred)
    *
    *	memory:			will keep track of previous values and will call any callback added
    *					after the list has been fired right away with the latest "memorized"
    *					values (like a Deferred)
    *
    *	unique:			will ensure a callback can only be added once (no duplicate in the list)
    *
    *	stopOnFalse:	interrupt callings when a callback returns false
    *
    */
    jQuery.Callbacks = function (flags) {

        // Convert flags from String-formatted to Object-formatted
        // (we check in cache first)
        flags = flags ? (flagsCache[flags] || createFlags(flags)) : {};

        var // Actual callback list
		list = [],
        // Stack of fire calls for repeatable lists
		stack = [],
        // Last fire value (for non-forgettable lists)
		memory,
        // Flag to know if list is currently firing
		firing,
        // First callback to fire (used internally by add and fireWith)
		firingStart,
        // End of the loop when firing
		firingLength,
        // Index of currently firing callback (modified by remove if needed)
		firingIndex,
        // Add one or several callbacks to the list
		add = function (args) {
		    var i,
				length,
				elem,
				type,
				actual;
		    for (i = 0, length = args.length; i < length; i++) {
		        elem = args[i];
		        type = jQuery.type(elem);
		        if (type === "array") {
		            // Inspect recursively
		            add(elem);
		        } else if (type === "function") {
		            // Add if not in unique mode and callback is not in
		            if (!flags.unique || !self.has(elem)) {
		                list.push(elem);
		            }
		        }
		    }
		},
        // Fire callbacks
		fire = function (context, args) {
		    args = args || [];
		    memory = !flags.memory || [context, args];
		    firing = true;
		    firingIndex = firingStart || 0;
		    firingStart = 0;
		    firingLength = list.length;
		    for (; list && firingIndex < firingLength; firingIndex++) {
		        if (list[firingIndex].apply(context, args) === false && flags.stopOnFalse) {
		            memory = true; // Mark as halted
		            break;
		        }
		    }
		    firing = false;
		    if (list) {
		        if (!flags.once) {
		            if (stack && stack.length) {
		                memory = stack.shift();
		                self.fireWith(memory[0], memory[1]);
		            }
		        } else if (memory === true) {
		            self.disable();
		        } else {
		            list = [];
		        }
		    }
		},
        // Actual Callbacks object
		self = {
		    // Add a callback or a collection of callbacks to the list
		    add: function () {
		        if (list) {
		            var length = list.length;
		            add(arguments);
		            // Do we need to add the callbacks to the
		            // current firing batch?
		            if (firing) {
		                firingLength = list.length;
		                // With memory, if we're not firing then
		                // we should call right away, unless previous
		                // firing was halted (stopOnFalse)
		            } else if (memory && memory !== true) {
		                firingStart = length;
		                fire(memory[0], memory[1]);
		            }
		        }
		        return this;
		    },
		    // Remove a callback from the list
		    remove: function () {
		        if (list) {
		            var args = arguments,
						argIndex = 0,
						argLength = args.length;
		            for (; argIndex < argLength; argIndex++) {
		                for (var i = 0; i < list.length; i++) {
		                    if (args[argIndex] === list[i]) {
		                        // Handle firingIndex and firingLength
		                        if (firing) {
		                            if (i <= firingLength) {
		                                firingLength--;
		                                if (i <= firingIndex) {
		                                    firingIndex--;
		                                }
		                            }
		                        }
		                        // Remove the element
		                        list.splice(i--, 1);
		                        // If we have some unicity property then
		                        // we only need to do this once
		                        if (flags.unique) {
		                            break;
		                        }
		                    }
		                }
		            }
		        }
		        return this;
		    },
		    // Control if a given callback is in the list
		    has: function (fn) {
		        if (list) {
		            var i = 0,
						length = list.length;
		            for (; i < length; i++) {
		                if (fn === list[i]) {
		                    return true;
		                }
		            }
		        }
		        return false;
		    },
		    // Remove all callbacks from the list
		    empty: function () {
		        list = [];
		        return this;
		    },
		    // Have the list do nothing anymore
		    disable: function () {
		        list = stack = memory = undefined;
		        return this;
		    },
		    // Is it disabled?
		    disabled: function () {
		        return !list;
		    },
		    // Lock the list in its current state
		    lock: function () {
		        stack = undefined;
		        if (!memory || memory === true) {
		            self.disable();
		        }
		        return this;
		    },
		    // Is it locked?
		    locked: function () {
		        return !stack;
		    },
		    // Call all callbacks with the given context and arguments
		    fireWith: function (context, args) {
		        if (stack) {
		            if (firing) {
		                if (!flags.once) {
		                    stack.push([context, args]);
		                }
		            } else if (!(flags.once && memory)) {
		                fire(context, args);
		            }
		        }
		        return this;
		    },
		    // Call all the callbacks with the given arguments
		    fire: function () {
		        self.fireWith(this, arguments);
		        return this;
		    },
		    // To know if the callbacks have already been called at least once
		    fired: function () {
		        return !!memory;
		    }
		};

        return self;
    };




    var // Static reference to slice
	sliceDeferred = [].slice;

    jQuery.extend({

        Deferred: function (func) {
            var doneList = jQuery.Callbacks("once memory"),
			failList = jQuery.Callbacks("once memory"),
			progressList = jQuery.Callbacks("memory"),
			state = "pending",
			lists = {
			    resolve: doneList,
			    reject: failList,
			    notify: progressList
			},
			promise = {
			    done: doneList.add,
			    fail: failList.add,
			    progress: progressList.add,

			    state: function () {
			        return state;
			    },

			    // Deprecated
			    isResolved: doneList.fired,
			    isRejected: failList.fired,

			    then: function (doneCallbacks, failCallbacks, progressCallbacks) {
			        deferred.done(doneCallbacks).fail(failCallbacks).progress(progressCallbacks);
			        return this;
			    },
			    always: function () {
			        deferred.done.apply(deferred, arguments).fail.apply(deferred, arguments);
			        return this;
			    },
			    pipe: function (fnDone, fnFail, fnProgress) {
			        return jQuery.Deferred(function (newDefer) {
			            jQuery.each({
			                done: [fnDone, "resolve"],
			                fail: [fnFail, "reject"],
			                progress: [fnProgress, "notify"]
			            }, function (handler, data) {
			                var fn = data[0],
								action = data[1],
								returned;
			                if (jQuery.isFunction(fn)) {
			                    deferred[handler](function () {
			                        returned = fn.apply(this, arguments);
			                        if (returned && jQuery.isFunction(returned.promise)) {
			                            returned.promise().then(newDefer.resolve, newDefer.reject, newDefer.notify);
			                        } else {
			                            newDefer[action + "With"](this === deferred ? newDefer : this, [returned]);
			                        }
			                    });
			                } else {
			                    deferred[handler](newDefer[action]);
			                }
			            });
			        }).promise();
			    },
			    // Get a promise for this deferred
			    // If obj is provided, the promise aspect is added to the object
			    promise: function (obj) {
			        if (obj == null) {
			            obj = promise;
			        } else {
			            for (var key in promise) {
			                obj[key] = promise[key];
			            }
			        }
			        return obj;
			    }
			},
			deferred = promise.promise({}),
			key;

            for (key in lists) {
                deferred[key] = lists[key].fire;
                deferred[key + "With"] = lists[key].fireWith;
            }

            // Handle state
            deferred.done(function () {
                state = "resolved";
            }, failList.disable, progressList.lock).fail(function () {
                state = "rejected";
            }, doneList.disable, progressList.lock);

            // Call given func if any
            if (func) {
                func.call(deferred, deferred);
            }

            // All done!
            return deferred;
        },

        // Deferred helper
        when: function (firstParam) {
            var args = sliceDeferred.call(arguments, 0),
			i = 0,
			length = args.length,
			pValues = new Array(length),
			count = length,
			pCount = length,
			deferred = length <= 1 && firstParam && jQuery.isFunction(firstParam.promise) ?
				firstParam :
				jQuery.Deferred(),
			promise = deferred.promise();
            function resolveFunc(i) {
                return function (value) {
                    args[i] = arguments.length > 1 ? sliceDeferred.call(arguments, 0) : value;
                    if (!(--count)) {
                        deferred.resolveWith(deferred, args);
                    }
                };
            }
            function progressFunc(i) {
                return function (value) {
                    pValues[i] = arguments.length > 1 ? sliceDeferred.call(arguments, 0) : value;
                    deferred.notifyWith(promise, pValues);
                };
            }
            if (length > 1) {
                for (; i < length; i++) {
                    if (args[i] && args[i].promise && jQuery.isFunction(args[i].promise)) {
                        args[i].promise().then(resolveFunc(i), deferred.reject, progressFunc(i));
                    } else {
                        --count;
                    }
                }
                if (!count) {
                    deferred.resolveWith(deferred, args);
                }
            } else if (deferred !== firstParam) {
                deferred.resolveWith(deferred, length ? [firstParam] : []);
            }
            return promise;
        }
    });




    jQuery.support = (function () {

        var support,
		all,
		a,
		select,
		opt,
		input,
		marginDiv,
		fragment,
		tds,
		events,
		eventName,
		i,
		isSupported,
		div = document.createElement("div"),
		documentElement = document.documentElement;

        // Preliminary tests
        div.setAttribute("className", "t");
        div.innerHTML = "   <link/><table></table><a href='/a' style='top:1px;float:left;opacity:.55;'>a</a><input type='checkbox'/>";

        all = div.getElementsByTagName("*");
        a = div.getElementsByTagName("a")[0];

        // Can't get basic test support
        if (!all || !all.length || !a) {
            return {};
        }

        // First batch of supports tests
        select = document.createElement("select");
        opt = select.appendChild(document.createElement("option"));
        input = div.getElementsByTagName("input")[0];

        support = {
            // IE strips leading whitespace when .innerHTML is used
            leadingWhitespace: (div.firstChild.nodeType === 3),

            // Make sure that tbody elements aren't automatically inserted
            // IE will insert them into empty tables
            tbody: !div.getElementsByTagName("tbody").length,

            // Make sure that link elements get serialized correctly by innerHTML
            // This requires a wrapper element in IE
            htmlSerialize: !!div.getElementsByTagName("link").length,

            // Get the style information from getAttribute
            // (IE uses .cssText instead)
            style: /top/.test(a.getAttribute("style")),

            // Make sure that URLs aren't manipulated
            // (IE normalizes it by default)
            hrefNormalized: (a.getAttribute("href") === "/a"),

            // Make sure that element opacity exists
            // (IE uses filter instead)
            // Use a regex to work around a WebKit issue. See #5145
            opacity: /^0.55/.test(a.style.opacity),

            // Verify style float existence
            // (IE uses styleFloat instead of cssFloat)
            cssFloat: !!a.style.cssFloat,

            // Make sure that if no value is specified for a checkbox
            // that it defaults to "on".
            // (WebKit defaults to "" instead)
            checkOn: (input.value === "on"),

            // Make sure that a selected-by-default option has a working selected property.
            // (WebKit defaults to false instead of true, IE too, if it's in an optgroup)
            optSelected: opt.selected,

            // Test setAttribute on camelCase class. If it works, we need attrFixes when doing get/setAttribute (ie6/7)
            getSetAttribute: div.className !== "t",

            // Tests for enctype support on a form(#6743)
            enctype: !!document.createElement("form").enctype,

            // Makes sure cloning an html5 element does not cause problems
            // Where outerHTML is undefined, this still works
            html5Clone: document.createElement("nav").cloneNode(true).outerHTML !== "<:nav></:nav>",

            // Will be defined later
            submitBubbles: true,
            changeBubbles: true,
            focusinBubbles: false,
            deleteExpando: true,
            noCloneEvent: true,
            inlineBlockNeedsLayout: false,
            shrinkWrapBlocks: false,
            reliableMarginRight: true
        };

        // Make sure checked status is properly cloned
        input.checked = true;
        support.noCloneChecked = input.cloneNode(true).checked;

        // Make sure that the options inside disabled selects aren't marked as disabled
        // (WebKit marks them as disabled)
        select.disabled = true;
        support.optDisabled = !opt.disabled;

        // Test to see if it's possible to delete an expando from an element
        // Fails in Internet Explorer
        try {
            delete div.test;
        } catch (e) {
            support.deleteExpando = false;
        }

        if (!div.addEventListener && div.attachEvent && div.fireEvent) {
            div.attachEvent("onclick", function () {
                // Cloning a node shouldn't copy over any
                // bound event handlers (IE does this)
                support.noCloneEvent = false;
            });
            div.cloneNode(true).fireEvent("onclick");
        }

        // Check if a radio maintains its value
        // after being appended to the DOM
        input = document.createElement("input");
        input.value = "t";
        input.setAttribute("type", "radio");
        support.radioValue = input.value === "t";

        input.setAttribute("checked", "checked");
        div.appendChild(input);
        fragment = document.createDocumentFragment();
        fragment.appendChild(div.lastChild);

        // WebKit doesn't clone checked state correctly in fragments
        support.checkClone = fragment.cloneNode(true).cloneNode(true).lastChild.checked;

        // Check if a disconnected checkbox will retain its checked
        // value of true after appended to the DOM (IE6/7)
        support.appendChecked = input.checked;

        fragment.removeChild(input);
        fragment.appendChild(div);

        div.innerHTML = "";

        // Check if div with explicit width and no margin-right incorrectly
        // gets computed margin-right based on width of container. For more
        // info see bug #3333
        // Fails in WebKit before Feb 2011 nightlies
        // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
        if (window.getComputedStyle) {
            marginDiv = document.createElement("div");
            marginDiv.style.width = "0";
            marginDiv.style.marginRight = "0";
            div.style.width = "2px";
            div.appendChild(marginDiv);
            support.reliableMarginRight =
			(parseInt((window.getComputedStyle(marginDiv, null) || { marginRight: 0 }).marginRight, 10) || 0) === 0;
        }

        // Technique from Juriy Zaytsev
        // http://perfectionkills.com/detecting-event-support-without-browser-sniffing/
        // We only care about the case where non-standard event systems
        // are used, namely in IE. Short-circuiting here helps us to
        // avoid an eval call (in setAttribute) which can cause CSP
        // to go haywire. See: https://developer.mozilla.org/en/Security/CSP
        if (div.attachEvent) {
            for (i in {
                submit: 1,
                change: 1,
                focusin: 1
            }) {
                eventName = "on" + i;
                isSupported = (eventName in div);
                if (!isSupported) {
                    div.setAttribute(eventName, "return;");
                    isSupported = (typeof div[eventName] === "function");
                }
                support[i + "Bubbles"] = isSupported;
            }
        }

        fragment.removeChild(div);

        // Null elements to avoid leaks in IE
        fragment = select = opt = marginDiv = div = input = null;

        // Run tests that need a body at doc ready
        jQuery(function () {
            var container, outer, inner, table, td, offsetSupport,
			conMarginTop, ptlm, vb, style, html,
			body = document.getElementsByTagName("body")[0];

            if (!body) {
                // Return for frameset docs that don't have a body
                return;
            }

            conMarginTop = 1;
            ptlm = "position:absolute;top:0;left:0;width:1px;height:1px;margin:0;";
            vb = "visibility:hidden;border:0;";
            style = "style='" + ptlm + "border:5px solid #000;padding:0;'";
            html = "<div " + style + "><div></div></div>" +
			"<table " + style + " cellpadding='0' cellspacing='0'>" +
			"<tr><td></td></tr></table>";

            container = document.createElement("div");
            container.style.cssText = vb + "width:0;height:0;position:static;top:0;margin-top:" + conMarginTop + "px";
            body.insertBefore(container, body.firstChild);

            // Construct the test element
            div = document.createElement("div");
            container.appendChild(div);

            // Check if table cells still have offsetWidth/Height when they are set
            // to display:none and there are still other visible table cells in a
            // table row; if so, offsetWidth/Height are not reliable for use when
            // determining if an element has been hidden directly using
            // display:none (it is still safe to use offsets if a parent element is
            // hidden; don safety goggles and see bug #4512 for more information).
            // (only IE 8 fails this test)
            div.innerHTML = "<table><tr><td style='padding:0;border:0;display:none'></td><td>t</td></tr></table>";
            tds = div.getElementsByTagName("td");
            isSupported = (tds[0].offsetHeight === 0);

            tds[0].style.display = "";
            tds[1].style.display = "none";

            // Check if empty table cells still have offsetWidth/Height
            // (IE <= 8 fail this test)
            support.reliableHiddenOffsets = isSupported && (tds[0].offsetHeight === 0);

            // Figure out if the W3C box model works as expected
            div.innerHTML = "";
            div.style.width = div.style.paddingLeft = "1px";
            jQuery.boxModel = support.boxModel = div.offsetWidth === 2;

            if (typeof div.style.zoom !== "undefined") {
                // Check if natively block-level elements act like inline-block
                // elements when setting their display to 'inline' and giving
                // them layout
                // (IE < 8 does this)
                div.style.display = "inline";
                div.style.zoom = 1;
                support.inlineBlockNeedsLayout = (div.offsetWidth === 2);

                // Check if elements with layout shrink-wrap their children
                // (IE 6 does this)
                div.style.display = "";
                div.innerHTML = "<div style='width:4px;'></div>";
                support.shrinkWrapBlocks = (div.offsetWidth !== 2);
            }

            div.style.cssText = ptlm + vb;
            div.innerHTML = html;

            outer = div.firstChild;
            inner = outer.firstChild;
            td = outer.nextSibling.firstChild.firstChild;

            offsetSupport = {
                doesNotAddBorder: (inner.offsetTop !== 5),
                doesAddBorderForTableAndCells: (td.offsetTop === 5)
            };

            inner.style.position = "fixed";
            inner.style.top = "20px";

            // safari subtracts parent border width here which is 5px
            offsetSupport.fixedPosition = (inner.offsetTop === 20 || inner.offsetTop === 15);
            inner.style.position = inner.style.top = "";

            outer.style.overflow = "hidden";
            outer.style.position = "relative";

            offsetSupport.subtractsBorderForOverflowNotVisible = (inner.offsetTop === -5);
            offsetSupport.doesNotIncludeMarginInBodyOffset = (body.offsetTop !== conMarginTop);

            body.removeChild(container);
            div = container = null;

            jQuery.extend(support, offsetSupport);
        });

        return support;
    })();




    var rbrace = /^(?:\{.*\}|\[.*\])$/,
	rmultiDash = /([A-Z])/g;

    jQuery.extend({
        cache: {},

        // Please use with caution
        uuid: 0,

        // Unique for each copy of jQuery on the page
        // Non-digits removed to match rinlinejQuery
        expando: "jQuery" + (jQuery.fn.jquery + Math.random()).replace(/\D/g, ""),

        // The following elements throw uncatchable exceptions if you
        // attempt to add expando properties to them.
        noData: {
            "embed": true,
            // Ban all objects except for Flash (which handle expandos)
            "object": "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
            "applet": true
        },

        hasData: function (elem) {
            elem = elem.nodeType ? jQuery.cache[elem[jQuery.expando]] : elem[jQuery.expando];
            return !!elem && !isEmptyDataObject(elem);
        },

        data: function (elem, name, data, pvt /* Internal Use Only */) {
            if (!jQuery.acceptData(elem)) {
                return;
            }

            var privateCache, thisCache, ret,
			internalKey = jQuery.expando,
			getByName = typeof name === "string",

            // We have to handle DOM nodes and JS objects differently because IE6-7
            // can't GC object references properly across the DOM-JS boundary
			isNode = elem.nodeType,

            // Only DOM nodes need the global jQuery cache; JS object data is
            // attached directly to the object so GC can occur automatically
			cache = isNode ? jQuery.cache : elem,

            // Only defining an ID for JS objects if its cache already exists allows
            // the code to shortcut on the same path as a DOM node with no cache
			id = isNode ? elem[internalKey] : elem[internalKey] && internalKey,
			isEvents = name === "events";

            // Avoid doing any more work than we need to when trying to get data on an
            // object that has no data at all
            if ((!id || !cache[id] || (!isEvents && !pvt && !cache[id].data)) && getByName && data === undefined) {
                return;
            }

            if (!id) {
                // Only DOM nodes need a new unique ID for each element since their data
                // ends up in the global cache
                if (isNode) {
                    elem[internalKey] = id = ++jQuery.uuid;
                } else {
                    id = internalKey;
                }
            }

            if (!cache[id]) {
                cache[id] = {};

                // Avoids exposing jQuery metadata on plain JS objects when the object
                // is serialized using JSON.stringify
                if (!isNode) {
                    cache[id].toJSON = jQuery.noop;
                }
            }

            // An object can be passed to jQuery.data instead of a key/value pair; this gets
            // shallow copied over onto the existing cache
            if (typeof name === "object" || typeof name === "function") {
                if (pvt) {
                    cache[id] = jQuery.extend(cache[id], name);
                } else {
                    cache[id].data = jQuery.extend(cache[id].data, name);
                }
            }

            privateCache = thisCache = cache[id];

            // jQuery data() is stored in a separate object inside the object's internal data
            // cache in order to avoid key collisions between internal data and user-defined
            // data.
            if (!pvt) {
                if (!thisCache.data) {
                    thisCache.data = {};
                }

                thisCache = thisCache.data;
            }

            if (data !== undefined) {
                thisCache[jQuery.camelCase(name)] = data;
            }

            // Users should not attempt to inspect the internal events object using jQuery.data,
            // it is undocumented and subject to change. But does anyone listen? No.
            if (isEvents && !thisCache[name]) {
                return privateCache.events;
            }

            // Check for both converted-to-camel and non-converted data property names
            // If a data property was specified
            if (getByName) {

                // First Try to find as-is property data
                ret = thisCache[name];

                // Test for null|undefined property data
                if (ret == null) {

                    // Try to find the camelCased property
                    ret = thisCache[jQuery.camelCase(name)];
                }
            } else {
                ret = thisCache;
            }

            return ret;
        },

        removeData: function (elem, name, pvt /* Internal Use Only */) {
            if (!jQuery.acceptData(elem)) {
                return;
            }

            var thisCache, i, l,

            // Reference to internal data cache key
			internalKey = jQuery.expando,

			isNode = elem.nodeType,

            // See jQuery.data for more information
			cache = isNode ? jQuery.cache : elem,

            // See jQuery.data for more information
			id = isNode ? elem[internalKey] : internalKey;

            // If there is already no cache entry for this object, there is no
            // purpose in continuing
            if (!cache[id]) {
                return;
            }

            if (name) {

                thisCache = pvt ? cache[id] : cache[id].data;

                if (thisCache) {

                    // Support array or space separated string names for data keys
                    if (!jQuery.isArray(name)) {

                        // try the string as a key before any manipulation
                        if (name in thisCache) {
                            name = [name];
                        } else {

                            // split the camel cased version by spaces unless a key with the spaces exists
                            name = jQuery.camelCase(name);
                            if (name in thisCache) {
                                name = [name];
                            } else {
                                name = name.split(" ");
                            }
                        }
                    }

                    for (i = 0, l = name.length; i < l; i++) {
                        delete thisCache[name[i]];
                    }

                    // If there is no data left in the cache, we want to continue
                    // and let the cache object itself get destroyed
                    if (!(pvt ? isEmptyDataObject : jQuery.isEmptyObject)(thisCache)) {
                        return;
                    }
                }
            }

            // See jQuery.data for more information
            if (!pvt) {
                delete cache[id].data;

                // Don't destroy the parent cache unless the internal data object
                // had been the only thing left in it
                if (!isEmptyDataObject(cache[id])) {
                    return;
                }
            }

            // Browsers that fail expando deletion also refuse to delete expandos on
            // the window, but it will allow it on all other JS objects; other browsers
            // don't care
            // Ensure that `cache` is not a window object #10080
            if (jQuery.support.deleteExpando || !cache.setInterval) {
                delete cache[id];
            } else {
                cache[id] = null;
            }

            // We destroyed the cache and need to eliminate the expando on the node to avoid
            // false lookups in the cache for entries that no longer exist
            if (isNode) {
                // IE does not allow us to delete expando properties from nodes,
                // nor does it have a removeAttribute function on Document nodes;
                // we must handle all of these cases
                if (jQuery.support.deleteExpando) {
                    delete elem[internalKey];
                } else if (elem.removeAttribute) {
                    elem.removeAttribute(internalKey);
                } else {
                    elem[internalKey] = null;
                }
            }
        },

        // For internal use only.
        _data: function (elem, name, data) {
            return jQuery.data(elem, name, data, true);
        },

        // A method for determining if a DOM node can handle the data expando
        acceptData: function (elem) {
            if (elem.nodeName) {
                var match = jQuery.noData[elem.nodeName.toLowerCase()];

                if (match) {
                    return !(match === true || elem.getAttribute("classid") !== match);
                }
            }

            return true;
        }
    });

    jQuery.fn.extend({
        data: function (key, value) {
            var parts, attr, name,
			data = null;

            if (typeof key === "undefined") {
                if (this.length) {
                    data = jQuery.data(this[0]);

                    if (this[0].nodeType === 1 && !jQuery._data(this[0], "parsedAttrs")) {
                        attr = this[0].attributes;
                        for (var i = 0, l = attr.length; i < l; i++) {
                            name = attr[i].name;

                            if (name.indexOf("data-") === 0) {
                                name = jQuery.camelCase(name.substring(5));

                                dataAttr(this[0], name, data[name]);
                            }
                        }
                        jQuery._data(this[0], "parsedAttrs", true);
                    }
                }

                return data;

            } else if (typeof key === "object") {
                return this.each(function () {
                    jQuery.data(this, key);
                });
            }

            parts = key.split(".");
            parts[1] = parts[1] ? "." + parts[1] : "";

            if (value === undefined) {
                data = this.triggerHandler("getData" + parts[1] + "!", [parts[0]]);

                // Try to fetch any internally stored data first
                if (data === undefined && this.length) {
                    data = jQuery.data(this[0], key);
                    data = dataAttr(this[0], key, data);
                }

                return data === undefined && parts[1] ?
				this.data(parts[0]) :
				data;

            } else {
                return this.each(function () {
                    var self = jQuery(this),
					args = [parts[0], value];

                    self.triggerHandler("setData" + parts[1] + "!", args);
                    jQuery.data(this, key, value);
                    self.triggerHandler("changeData" + parts[1] + "!", args);
                });
            }
        },

        removeData: function (key) {
            return this.each(function () {
                jQuery.removeData(this, key);
            });
        }
    });

    function dataAttr(elem, key, data) {
        // If nothing was found internally, try to fetch any
        // data from the HTML5 data-* attribute
        if (data === undefined && elem.nodeType === 1) {

            var name = "data-" + key.replace(rmultiDash, "-$1").toLowerCase();

            data = elem.getAttribute(name);

            if (typeof data === "string") {
                try {
                    data = data === "true" ? true :
				data === "false" ? false :
				data === "null" ? null :
				jQuery.isNumeric(data) ? parseFloat(data) :
					rbrace.test(data) ? jQuery.parseJSON(data) :
					data;
                } catch (e) { }

                // Make sure we set the data so it isn't changed later
                jQuery.data(elem, key, data);

            } else {
                data = undefined;
            }
        }

        return data;
    }

    // checks a cache object for emptiness
    function isEmptyDataObject(obj) {
        for (var name in obj) {

            // if the public data object is empty, the private is still empty
            if (name === "data" && jQuery.isEmptyObject(obj[name])) {
                continue;
            }
            if (name !== "toJSON") {
                return false;
            }
        }

        return true;
    }




    function handleQueueMarkDefer(elem, type, src) {
        var deferDataKey = type + "defer",
		queueDataKey = type + "queue",
		markDataKey = type + "mark",
		defer = jQuery._data(elem, deferDataKey);
        if (defer &&
		(src === "queue" || !jQuery._data(elem, queueDataKey)) &&
		(src === "mark" || !jQuery._data(elem, markDataKey))) {
            // Give room for hard-coded callbacks to fire first
            // and eventually mark/queue something else on the element
            setTimeout(function () {
                if (!jQuery._data(elem, queueDataKey) &&
				!jQuery._data(elem, markDataKey)) {
                    jQuery.removeData(elem, deferDataKey, true);
                    defer.fire();
                }
            }, 0);
        }
    }

    jQuery.extend({

        _mark: function (elem, type) {
            if (!jv.IsNull(elem)) {
                type = (type || "fx") + "mark";
                jQuery._data(elem, type, (jQuery._data(elem, type) || 0) + 1);
            }
        },

        _unmark: function (force, elem, type) {
            if (force !== true) {
                type = elem;
                elem = force;
                force = false;
            }
            if (!jv.IsNull(elem)) {
                type = type || "fx";
                var key = type + "mark",
				count = force ? 0 : ((jQuery._data(elem, key) || 1) - 1);
                if (count) {
                    jQuery._data(elem, key, count);
                } else {
                    jQuery.removeData(elem, key, true);
                    handleQueueMarkDefer(elem, type, "mark");
                }
            }
        },

        queue: function (elem, type, data) {
            var q;
            if (!jv.IsNull(elem)) {
                type = (type || "fx") + "queue";
                q = jQuery._data(elem, type);

                // Speed up dequeue by getting out quickly if this is just a lookup
                if (data) {
                    if (!q || jQuery.isArray(data)) {
                        q = jQuery._data(elem, type, jQuery.makeArray(data));
                    } else {
                        q.push(data);
                    }
                }
                return q || [];
            }
        },

        dequeue: function (elem, type) {
            type = type || "fx";

            var queue = jQuery.queue(elem, type),
			fn = queue.shift(),
			hooks = {};

            // If the fx queue is dequeued, always remove the progress sentinel
            if (fn === "inprogress") {
                fn = queue.shift();
            }

            if (fn) {
                // Add a progress sentinel to prevent the fx queue from being
                // automatically dequeued
                if (type === "fx") {
                    queue.unshift("inprogress");
                }

                jQuery._data(elem, type + ".run", hooks);
                fn.call(elem, function () {
                    jQuery.dequeue(elem, type);
                }, hooks);
            }

            if (!queue.length) {
                jQuery.removeData(elem, type + "queue " + type + ".run", true);
                handleQueueMarkDefer(elem, type, "queue");
            }
        }
    });

    jQuery.fn.extend({
        queue: function (type, data) {
            if (typeof type !== "string") {
                data = type;
                type = "fx";
            }

            if (data === undefined) {
                return jQuery.queue(this[0], type);
            }
            return this.each(function () {
                var queue = jQuery.queue(this, type, data);

                if (type === "fx" && queue[0] !== "inprogress") {
                    jQuery.dequeue(this, type);
                }
            });
        },
        dequeue: function (type) {
            return this.each(function () {
                jQuery.dequeue(this, type);
            });
        },
        // Based off of the plugin by Clint Helfers, with permission.
        // http://blindsignals.com/index.php/2009/07/jquery-delay/
        delay: function (time, type) {
            time = jQuery.fx ? jQuery.fx.speeds[time] || time : time;
            type = type || "fx";

            return this.queue(type, function (next, hooks) {
                var timeout = setTimeout(next, time);
                hooks.stop = function () {
                    clearTimeout(timeout);
                };
            });
        },
        clearQueue: function (type) {
            return this.queue(type || "fx", []);
        },
        // Get a promise resolved when queues of a certain type
        // are emptied (fx is the type by default)
        promise: function (type, object) {
            if (typeof type !== "string") {
                object = type;
                type = undefined;
            }
            type = type || "fx";
            var defer = jQuery.Deferred(),
			elements = this,
			i = elements.length,
			count = 1,
			deferDataKey = type + "defer",
			queueDataKey = type + "queue",
			markDataKey = type + "mark",
			tmp;
            function resolve() {
                if (!(--count)) {
                    defer.resolveWith(elements, [elements]);
                }
            }
            while (i--) {
                if ((tmp = jQuery.data(elements[i], deferDataKey, undefined, true) ||
					(jQuery.data(elements[i], queueDataKey, undefined, true) ||
						jQuery.data(elements[i], markDataKey, undefined, true)) &&
					jQuery.data(elements[i], deferDataKey, jQuery.Callbacks("once memory"), true))) {
                    count++;
                    tmp.add(resolve);
                }
            }
            resolve();
            return defer.promise();
        }
    });




    var rclass = /[\n\t\r]/g,
	rspace = /\s+/,
	rreturn = /\r/g,
	rtype = /^(?:button|input)$/i,
	rfocusable = /^(?:button|input|object|select|textarea)$/i,
	rclickable = /^a(?:rea)?$/i,
	rboolean = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i,
	getSetAttribute = jQuery.support.getSetAttribute,
	nodeHook, boolHook, fixSpecified;

    jQuery.fn.extend({
        attr: function (name, value) {
            return jQuery.access(this, name, value, true, jQuery.attr);
        },

        removeAttr: function (name) {
            return this.each(function () {
                jQuery.removeAttr(this, name);
            });
        },

        prop: function (name, value) {
            return jQuery.access(this, name, value, true, jQuery.prop);
        },

        removeProp: function (name) {
            name = jQuery.propFix[name] || name;
            return this.each(function () {
                // try/catch handles cases where IE balks (such as removing a property on window)
                try {
                    this[name] = undefined;
                    delete this[name];
                } catch (e) { }
            });
        },

        addClass: function (value) {
            var classNames, i, l, elem,
			setClass, c, cl;

            if (jQuery.isFunction(value)) {
                return this.each(function (j) {
                    jQuery(this).addClass(value.call(this, j, this.className));
                });
            }

            if (value && typeof value === "string") {
                classNames = value.split(rspace);

                for (i = 0, l = this.length; i < l; i++) {
                    elem = this[i];

                    if (elem.nodeType === 1) {
                        if (!elem.className && classNames.length === 1) {
                            elem.className = value;

                        } else {
                            setClass = " " + elem.className + " ";

                            for (c = 0, cl = classNames.length; c < cl; c++) {
                                if (! ~setClass.indexOf(" " + classNames[c] + " ")) {
                                    setClass += classNames[c] + " ";
                                }
                            }
                            elem.className = jQuery.trim(setClass);
                        }
                    }
                }
            }

            return this;
        },

        removeClass: function (value) {
            var classNames, i, l, elem, className, c, cl;

            if (jQuery.isFunction(value)) {
                return this.each(function (j) {
                    jQuery(this).removeClass(value.call(this, j, this.className));
                });
            }

            if ((value && typeof value === "string") || value === undefined) {
                classNames = (value || "").split(rspace);

                for (i = 0, l = this.length; i < l; i++) {
                    elem = this[i];

                    if (elem.nodeType === 1 && elem.className) {
                        if (value) {
                            className = (" " + elem.className + " ").replace(rclass, " ");
                            for (c = 0, cl = classNames.length; c < cl; c++) {
                                className = className.replace(" " + classNames[c] + " ", " ");
                            }
                            elem.className = jQuery.trim(className);

                        } else {
                            elem.className = "";
                        }
                    }
                }
            }

            return this;
        },

        toggleClass: function (value, stateVal) {
            var type = typeof value,
			isBool = typeof stateVal === "boolean";

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    jQuery(this).toggleClass(value.call(this, i, this.className, stateVal), stateVal);
                });
            }

            return this.each(function () {
                if (type === "string") {
                    // toggle individual class names
                    var className,
					i = 0,
					self = jQuery(this),
					state = stateVal,
					classNames = value.split(rspace);

                    while ((className = classNames[i++])) {
                        // check each className given, space seperated list
                        state = isBool ? state : !self.hasClass(className);
                        self[state ? "addClass" : "removeClass"](className);
                    }

                } else if (type === "undefined" || type === "boolean") {
                    if (this.className) {
                        // store className if set
                        jQuery._data(this, "__className__", this.className);
                    }

                    // toggle whole className
                    this.className = this.className || value === false ? "" : jQuery._data(this, "__className__") || "";
                }
            });
        },

        hasClass: function (selector) {
            var className = " " + selector + " ",
			i = 0,
			l = this.length;
            for (; i < l; i++) {
                if (this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf(className) > -1) {
                    return true;
                }
            }

            return false;
        },

        val: function (value) {
            var hooks, ret, isFunction,
			elem = this[0];

            if (!arguments.length) {
                if (!jv.IsNull(elem)) {
                    hooks = jQuery.valHooks[elem.nodeName.toLowerCase()] || jQuery.valHooks[elem.type];

                    if (hooks && "get" in hooks && (ret = hooks.get(elem, "value")) !== undefined) {
                        return ret;
                    }

                    ret = elem.value;

                    return typeof ret === "string" ?
                    // handle most common string cases
					ret.replace(rreturn, "") :
                    // handle cases where value is null/undef or number
					ret == null ? "" : ret;
                }

                return;
            }

            isFunction = jQuery.isFunction(value);

            return this.each(function (i) {
                var self = jQuery(this), val;

                if (this.nodeType !== 1) {
                    return;
                }

                if (isFunction) {
                    val = value.call(this, i, self.val());
                } else {
                    val = value;
                }

                // Treat null/undefined as ""; convert numbers to string
                if (val == null) {
                    val = "";
                } else if (typeof val === "number") {
                    val += "";
                } else if (jQuery.isArray(val)) {
                    val = jQuery.map(val, function (value) {
                        return value == null ? "" : value + "";
                    });
                }

                hooks = jQuery.valHooks[this.nodeName.toLowerCase()] || jQuery.valHooks[this.type];

                // If set returns undefined, fall back to normal setting
                if (!hooks || !("set" in hooks) || hooks.set(this, val, "value") === undefined) {
                    this.value = val;
                }
            });
        }
    });

    jQuery.extend({
        valHooks: {
            option: {
                get: function (elem) {
                    // attributes.value is undefined in Blackberry 4.7 but
                    // uses .value. See #6932
                    var val = elem.attributes.value;
                    return !val || val.specified ? elem.value : elem.text;
                }
            },
            select: {
                get: function (elem) {
                    var value, i, max, option,
					index = elem.selectedIndex,
					values = [],
					options = elem.options,
					one = elem.type === "select-one";

                    // Nothing was selected
                    if (index < 0) {
                        return null;
                    }

                    // Loop through all the selected options
                    i = one ? index : 0;
                    max = one ? index + 1 : options.length;
                    for (; i < max; i++) {
                        option = options[i];

                        // Don't return options that are disabled or in a disabled optgroup
                        if (option.selected && (jQuery.support.optDisabled ? !option.disabled : option.getAttribute("disabled") === null) &&
							(!option.parentNode.disabled || !jQuery.nodeName(option.parentNode, "optgroup"))) {

                            // Get the specific value for the option
                            value = jQuery(option).val();

                            // We don't need an array for one selects
                            if (one) {
                                return value;
                            }

                            // Multi-Selects return an array
                            values.push(value);
                        }
                    }

                    // Fixes Bug #2551 -- select.val() broken in IE after form.reset()
                    if (one && !values.length && options.length) {
                        return jQuery(options[index]).val();
                    }

                    return values;
                },

                set: function (elem, value) {
                    var values = jQuery.makeArray(value);

                    jQuery(elem).find("option").each(function () {
                        this.selected = jQuery.inArray(jQuery(this).val(), values) >= 0;
                    });

                    if (!values.length) {
                        elem.selectedIndex = -1;
                    }
                    return values;
                }
            }
        },

        attrFn: {
            val: true,
            css: true,
            html: true,
            text: true,
            data: true,
            width: true,
            height: true,
            offset: true
        },

        attr: function (elem, name, value, pass) {
            var ret, hooks, notxml,
			nType = elem.nodeType;

            // don't get/set attributes on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            if (pass && name in jQuery.attrFn) {
                return jQuery(elem)[name](value);
            }

            // Fallback to prop when attributes are not supported
            if (typeof elem.getAttribute === "undefined") {
                return jQuery.prop(elem, name, value);
            }

            notxml = nType !== 1 || !jQuery.isXMLDoc(elem);

            // All attributes are lowercase
            // Grab necessary hook if one is defined
            if (notxml) {
                name = name.toLowerCase();
                hooks = jQuery.attrHooks[name] || (rboolean.test(name) ? boolHook : nodeHook);
            }

            if (value !== undefined) {

                if (value === null) {
                    jQuery.removeAttr(elem, name);
                    return;

                } else if (hooks && "set" in hooks && notxml && (ret = hooks.set(elem, value, name)) !== undefined) {
                    return ret;

                } else {
                    elem.setAttribute(name, "" + value);
                    return value;
                }

            } else if (hooks && "get" in hooks && notxml && (ret = hooks.get(elem, name)) !== null) {
                return ret;

            } else {

                ret = elem.getAttribute(name);

                // Non-existent attributes return null, we normalize to undefined
                return ret === null ?
				undefined :
				ret;
            }
        },

        removeAttr: function (elem, value) {
            var propName, attrNames, name, l,
			i = 0;

            if (value && elem.nodeType === 1) {
                attrNames = value.toLowerCase().split(rspace);
                l = attrNames.length;

                for (; i < l; i++) {
                    name = attrNames[i];

                    if (name) {
                        propName = jQuery.propFix[name] || name;

                        // See #9699 for explanation of this approach (setting first, then removal)
                        jQuery.attr(elem, name, "");
                        elem.removeAttribute(getSetAttribute ? name : propName);

                        // Set corresponding property to false for boolean attributes
                        if (rboolean.test(name) && propName in elem) {
                            elem[propName] = false;
                        }
                    }
                }
            }
        },

        attrHooks: {
            type: {
                set: function (elem, value) {
                    // We can't allow the type property to be changed (since it causes problems in IE)
                    if (rtype.test(elem.nodeName) && elem.parentNode) {
                        jQuery.error("type property can't be changed");
                    } else if (!jQuery.support.radioValue && value === "radio" && jQuery.nodeName(elem, "input")) {
                        // Setting the type on a radio button after the value resets the value in IE6-9
                        // Reset value to it's default in case type is set after value
                        // This is for element creation
                        var val = elem.value;
                        elem.setAttribute("type", value);
                        if (val) {
                            elem.value = val;
                        }
                        return value;
                    }
                }
            },
            // Use the value property for back compat
            // Use the nodeHook for button elements in IE6/7 (#1954)
            value: {
                get: function (elem, name) {
                    if (nodeHook && jQuery.nodeName(elem, "button")) {
                        return nodeHook.get(elem, name);
                    }
                    return name in elem ?
					elem.value :
					null;
                },
                set: function (elem, value, name) {
                    if (nodeHook && jQuery.nodeName(elem, "button")) {
                        return nodeHook.set(elem, value, name);
                    }
                    // Does not return so that setAttribute is also used
                    elem.value = value;
                }
            }
        },

        propFix: {
            tabindex: "tabIndex",
            readonly: "readOnly",
            "for": "htmlFor",
            "class": "className",
            maxlength: "maxLength",
            cellspacing: "cellSpacing",
            cellpadding: "cellPadding",
            rowspan: "rowSpan",
            colspan: "colSpan",
            usemap: "useMap",
            frameborder: "frameBorder",
            contenteditable: "contentEditable"
        },

        prop: function (elem, name, value) {
            var ret, hooks, notxml,
			nType = elem.nodeType;

            // don't get/set properties on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            notxml = nType !== 1 || !jQuery.isXMLDoc(elem);

            if (notxml) {
                // Fix name and attach hooks
                name = jQuery.propFix[name] || name;
                hooks = jQuery.propHooks[name];
            }

            if (value !== undefined) {
                if (hooks && "set" in hooks && (ret = hooks.set(elem, value, name)) !== undefined) {
                    return ret;

                } else {
                    return (elem[name] = value);
                }

            } else {
                if (hooks && "get" in hooks && (ret = hooks.get(elem, name)) !== null) {
                    return ret;

                } else {
                    return elem[name];
                }
            }
        },

        propHooks: {
            tabIndex: {
                get: function (elem) {
                    // elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
                    // http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
                    var attributeNode = elem.getAttributeNode("tabindex");

                    return attributeNode && attributeNode.specified ?
					parseInt(attributeNode.value, 10) :
					rfocusable.test(elem.nodeName) || rclickable.test(elem.nodeName) && elem.href ?
						0 :
						undefined;
                }
            }
        }
    });

    // Add the tabIndex propHook to attrHooks for back-compat (different case is intentional)
    jQuery.attrHooks.tabindex = jQuery.propHooks.tabIndex;

    // Hook for boolean attributes
    boolHook = {
        get: function (elem, name) {
            // Align boolean attributes with corresponding properties
            // Fall back to attribute presence where some booleans are not supported
            var attrNode,
			property = jQuery.prop(elem, name);
            return property === true || typeof property !== "boolean" && (attrNode = elem.getAttributeNode(name)) && attrNode.nodeValue !== false ?
			name.toLowerCase() :
			undefined;
        },
        set: function (elem, value, name) {
            var propName;
            if (value === false) {
                // Remove boolean attributes when set to false
                jQuery.removeAttr(elem, name);
            } else {
                // value is true since we know at this point it's type boolean and not false
                // Set boolean attributes to the same name and set the DOM property
                propName = jQuery.propFix[name] || name;
                if (propName in elem) {
                    // Only set the IDL specifically if it already exists on the element
                    elem[propName] = true;
                }

                elem.setAttribute(name, name.toLowerCase());
            }
            return name;
        }
    };

    // IE6/7 do not support getting/setting some attributes with get/setAttribute
    if (!getSetAttribute) {

        fixSpecified = {
            name: true,
            id: true
        };

        // Use this for any attribute in IE6/7
        // This fixes almost every IE6/7 issue
        nodeHook = jQuery.valHooks.button = {
            get: function (elem, name) {
                var ret;
                ret = elem.getAttributeNode(name);
                return ret && (fixSpecified[name] ? ret.nodeValue !== "" : ret.specified) ?
				ret.nodeValue :
				undefined;
            },
            set: function (elem, value, name) {
                // Set the existing or create a new attribute node
                var ret = elem.getAttributeNode(name);
                if (!ret) {
                    ret = document.createAttribute(name);
                    elem.setAttributeNode(ret);
                }
                return (ret.nodeValue = value + "");
            }
        };

        // Apply the nodeHook to tabindex
        jQuery.attrHooks.tabindex.set = nodeHook.set;

        // Set width and height to auto instead of 0 on empty string( Bug #8150 )
        // This is for removals
        jQuery.each(["width", "height"], function (i, name) {
            jQuery.attrHooks[name] = jQuery.extend(jQuery.attrHooks[name], {
                set: function (elem, value) {
                    if (value === "") {
                        elem.setAttribute(name, "auto");
                        return value;
                    }
                }
            });
        });

        // Set contenteditable to false on removals(#10429)
        // Setting to empty string throws an error as an invalid value
        jQuery.attrHooks.contenteditable = {
            get: nodeHook.get,
            set: function (elem, value, name) {
                if (value === "") {
                    value = "false";
                }
                nodeHook.set(elem, value, name);
            }
        };
    }


    // Some attributes require a special call on IE
    if (!jQuery.support.hrefNormalized) {
        jQuery.each(["href", "src", "width", "height"], function (i, name) {
            jQuery.attrHooks[name] = jQuery.extend(jQuery.attrHooks[name], {
                get: function (elem) {
                    var ret = elem.getAttribute(name, 2);
                    return ret === null ? undefined : ret;
                }
            });
        });
    }

    if (!jQuery.support.style) {
        jQuery.attrHooks.style = {
            get: function (elem) {
                // Return undefined in the case of empty string
                // Normalize to lowercase since IE uppercases css property names
                return elem.style.cssText.toLowerCase() || undefined;
            },
            set: function (elem, value) {
                return (elem.style.cssText = "" + value);
            }
        };
    }

    // Safari mis-reports the default selected property of an option
    // Accessing the parent's selectedIndex property fixes it
    if (!jQuery.support.optSelected) {
        jQuery.propHooks.selected = jQuery.extend(jQuery.propHooks.selected, {
            get: function (elem) {
                var parent = elem.parentNode;

                if (parent) {
                    parent.selectedIndex;

                    // Make sure that it also works with optgroups, see #5701
                    if (parent.parentNode) {
                        parent.parentNode.selectedIndex;
                    }
                }
                return null;
            }
        });
    }

    // IE6/7 call enctype encoding
    if (!jQuery.support.enctype) {
        jQuery.propFix.enctype = "encoding";
    }

    // Radios and checkboxes getter/setter
    if (!jQuery.support.checkOn) {
        jQuery.each(["radio", "checkbox"], function () {
            jQuery.valHooks[this] = {
                get: function (elem) {
                    // Handle the case where in Webkit "" is returned instead of "on" if a value isn't specified
                    return elem.getAttribute("value") === null ? "on" : elem.value;
                }
            };
        });
    }
    jQuery.each(["radio", "checkbox"], function () {
        jQuery.valHooks[this] = jQuery.extend(jQuery.valHooks[this], {
            set: function (elem, value) {
                if (jQuery.isArray(value)) {
                    return (elem.checked = jQuery.inArray(jQuery(elem).val(), value) >= 0);
                }
            }
        });
    });




    var rformElems = /^(?:textarea|input|select)$/i,
	rtypenamespace = /^([^\.]*)?(?:\.(.+))?$/,
	rhoverHack = /\bhover(\.\S+)?\b/,
	rkeyEvent = /^key/,
	rmouseEvent = /^(?:mouse|contextmenu)|click/,
	rfocusMorph = /^(?:focusinfocus|focusoutblur)$/,
	rquickIs = /^(\w*)(?:#([\w\-]+))?(?:\.([\w\-]+))?$/,
	quickParse = function (selector) {
	    var quick = rquickIs.exec(selector);
	    if (quick) {
	        //   0  1    2   3
	        // [ _, tag, id, class ]
	        quick[1] = (quick[1] || "").toLowerCase();
	        quick[3] = quick[3] && new RegExp("(?:^|\\s)" + quick[3] + "(?:\\s|$)");
	    }
	    return quick;
	},
	quickIs = function (elem, m) {
	    var attrs = elem.attributes || {};
	    return (
			(!m[1] || elem.nodeName.toLowerCase() === m[1]) &&
			(!m[2] || (attrs.id || {}).value === m[2]) &&
			(!m[3] || m[3].test((attrs["class"] || {}).value))
		);
	},
	hoverHack = function (events) {
	    return jQuery.event.special.hover ? events : events.replace(rhoverHack, "mouseenter$1 mouseleave$1");
	};

    /*
    * Helper functions for managing events -- not part of the public interface.
    * Props to Dean Edwards' addEvent library for many of the ideas.
    */
    jQuery.event = {

        add: function (elem, types, handler, data, selector) {

            var elemData, eventHandle, events,
			t, tns, type, namespaces, handleObj,
			handleObjIn, quick, handlers, special;

            // Don't attach events to noData or text/comment nodes (allow plain objects tho)
            if (elem.nodeType === 3 || elem.nodeType === 8 || !types || !handler || !(elemData = jQuery._data(elem))) {
                return;
            }

            // Caller can pass in an object of custom data in lieu of the handler
            if (handler.handler) {
                handleObjIn = handler;
                handler = handleObjIn.handler;
            }

            // Make sure that the handler has a unique ID, used to find/remove it later
            if (!handler.guid) {
                handler.guid = jQuery.guid++;
            }

            // Init the element's event structure and main handler, if this is the first
            events = elemData.events;
            if (!events) {
                elemData.events = events = {};
            }
            eventHandle = elemData.handle;
            if (!eventHandle) {
                elemData.handle = eventHandle = function (e) {
                    // Discard the second event of a jQuery.event.trigger() and
                    // when an event is called after a page has unloaded
                    return typeof jQuery !== "undefined" && (!e || jQuery.event.triggered !== e.type) ?
					jQuery.event.dispatch.apply(eventHandle.elem, arguments) :
					undefined;
                };
                // Add elem as a property of the handle fn to prevent a memory leak with IE non-native events
                eventHandle.elem = elem;
            }

            // Handle multiple events separated by a space
            // jQuery(...).bind("mouseover mouseout", fn);
            types = jQuery.trim(hoverHack(types)).split(" ");
            for (t = 0; t < types.length; t++) {

                tns = rtypenamespace.exec(types[t]) || [];
                type = tns[1];
                namespaces = (tns[2] || "").split(".").sort();

                // If event changes its type, use the special event handlers for the changed type
                special = jQuery.event.special[type] || {};

                // If selector defined, determine special event api type, otherwise given type
                type = (selector ? special.delegateType : special.bindType) || type;

                // Update special based on newly reset type
                special = jQuery.event.special[type] || {};

                // handleObj is passed to all event handlers
                handleObj = jQuery.extend({
                    type: type,
                    origType: tns[1],
                    data: data,
                    handler: handler,
                    guid: handler.guid,
                    selector: selector,
                    quick: quickParse(selector),
                    namespace: namespaces.join(".")
                }, handleObjIn);

                // Init the event handler queue if we're the first
                handlers = events[type];
                if (!handlers) {
                    handlers = events[type] = [];
                    handlers.delegateCount = 0;

                    // Only use addEventListener/attachEvent if the special events handler returns false
                    if (!special.setup || special.setup.call(elem, data, namespaces, eventHandle) === false) {
                        // Bind the global event handler to the element
                        if (elem.addEventListener) {
                            elem.addEventListener(type, eventHandle, false);

                        } else if (elem.attachEvent) {
                            elem.attachEvent("on" + type, eventHandle);
                        }
                    }
                }

                if (special.add) {
                    special.add.call(elem, handleObj);

                    if (!handleObj.handler.guid) {
                        handleObj.handler.guid = handler.guid;
                    }
                }

                // Add to the element's handler list, delegates in front
                if (selector) {
                    handlers.splice(handlers.delegateCount++, 0, handleObj);
                } else {
                    handlers.push(handleObj);
                }

                // Keep track of which events have ever been used, for event optimization
                jQuery.event.global[type] = true;
            }

            // Nullify elem to prevent memory leaks in IE
            elem = null;
        },

        global: {},

        // Detach an event or set of events from an element
        remove: function (elem, types, handler, selector, mappedTypes) {

            var elemData = jQuery.hasData(elem) && jQuery._data(elem),
			t, tns, type, origType, namespaces, origCount,
			j, events, special, handle, eventType, handleObj;

            if (!elemData || !(events = elemData.events)) {
                return;
            }

            // Once for each type.namespace in types; type may be omitted
            types = jQuery.trim(hoverHack(types || "")).split(" ");
            for (t = 0; t < types.length; t++) {
                tns = rtypenamespace.exec(types[t]) || [];
                type = origType = tns[1];
                namespaces = tns[2];

                // Unbind all events (on this namespace, if provided) for the element
                if (!type) {
                    for (type in events) {
                        jQuery.event.remove(elem, type + types[t], handler, selector, true);
                    }
                    continue;
                }

                special = jQuery.event.special[type] || {};
                type = (selector ? special.delegateType : special.bindType) || type;
                eventType = events[type] || [];
                origCount = eventType.length;
                namespaces = namespaces ? new RegExp("(^|\\.)" + namespaces.split(".").sort().join("\\.(?:.*\\.)?") + "(\\.|$)") : null;

                // Remove matching events
                for (j = 0; j < eventType.length; j++) {
                    handleObj = eventType[j];

                    if ((mappedTypes || origType === handleObj.origType) &&
					 (!handler || handler.guid === handleObj.guid) &&
					 (!namespaces || namespaces.test(handleObj.namespace)) &&
					 (!selector || selector === handleObj.selector || selector === "**" && handleObj.selector)) {
                        eventType.splice(j--, 1);

                        if (handleObj.selector) {
                            eventType.delegateCount--;
                        }
                        if (special.remove) {
                            special.remove.call(elem, handleObj);
                        }
                    }
                }

                // Remove generic event handler if we removed something and no more handlers exist
                // (avoids potential for endless recursion during removal of special event handlers)
                if (eventType.length === 0 && origCount !== eventType.length) {
                    if (!special.teardown || special.teardown.call(elem, namespaces) === false) {
                        jQuery.removeEvent(elem, type, elemData.handle);
                    }

                    delete events[type];
                }
            }

            // Remove the expando if it's no longer used
            if (jQuery.isEmptyObject(events)) {
                handle = elemData.handle;
                if (handle) {
                    handle.elem = null;
                }

                // removeData also checks for emptiness and clears the expando if empty
                // so use it instead of delete
                jQuery.removeData(elem, ["events", "handle"], true);
            }
        },

        // Events that are safe to short-circuit if no handlers are attached.
        // Native DOM events should not be added, they may have inline handlers.
        customEvent: {
            "getData": true,
            "setData": true,
            "changeData": true
        },

        trigger: function (event, data, elem, onlyHandlers) {
            // Don't do events on text and comment nodes
            if (elem && (elem.nodeType === 3 || elem.nodeType === 8)) {
                return;
            }

            // Event object or event type
            var type = event.type || event,
			namespaces = [],
			cache, exclusive, i, cur, old, ontype, special, handle, eventPath, bubbleType;

            // focus/blur morphs to focusin/out; ensure we're not firing them right now
            if (rfocusMorph.test(type + jQuery.event.triggered)) {
                return;
            }

            if (type.indexOf("!") >= 0) {
                // Exclusive events trigger only for the exact event (no namespaces)
                type = type.slice(0, -1);
                exclusive = true;
            }

            if (type.indexOf(".") >= 0) {
                // Namespaced trigger; create a regexp to match event type in handle()
                namespaces = type.split(".");
                type = namespaces.shift();
                namespaces.sort();
            }

            if ((!elem || jQuery.event.customEvent[type]) && !jQuery.event.global[type]) {
                // No jQuery handlers for this event type, and it can't have inline handlers
                return;
            }

            // Caller can pass in an Event, Object, or just an event type string
            event = typeof event === "object" ?
            // jQuery.Event object
			event[jQuery.expando] ? event :
            // Object literal
			new jQuery.Event(type, event) :
            // Just the event type (string)
			new jQuery.Event(type);

            event.type = type;
            event.isTrigger = true;
            event.exclusive = exclusive;
            event.namespace = namespaces.join(".");
            event.namespace_re = event.namespace ? new RegExp("(^|\\.)" + namespaces.join("\\.(?:.*\\.)?") + "(\\.|$)") : null;
            ontype = type.indexOf(":") < 0 ? "on" + type : "";

            // Handle a global trigger
            if (!elem) {

                // TODO: Stop taunting the data cache; remove global events and always attach to document
                cache = jQuery.cache;
                for (i in cache) {
                    if (cache[i].events && cache[i].events[type]) {
                        jQuery.event.trigger(event, data, cache[i].handle.elem, true);
                    }
                }
                return;
            }

            // Clean up the event in case it is being reused
            event.result = undefined;
            if (!event.target) {
                event.target = elem;
            }

            // Clone any incoming data and prepend the event, creating the handler arg list
            data = data != null ? jQuery.makeArray(data) : [];
            data.unshift(event);

            // Allow special events to draw outside the lines
            special = jQuery.event.special[type] || {};
            if (special.trigger && special.trigger.apply(elem, data) === false) {
                return;
            }

            // Determine event propagation path in advance, per W3C events spec (#9951)
            // Bubble up to document, then to window; watch for a global ownerDocument var (#9724)
            eventPath = [[elem, special.bindType || type]];
            if (!onlyHandlers && !special.noBubble && !jQuery.isWindow(elem)) {

                bubbleType = special.delegateType || type;
                cur = rfocusMorph.test(bubbleType + type) ? elem : elem.parentNode;
                old = null;
                for (; cur; cur = cur.parentNode) {
                    eventPath.push([cur, bubbleType]);
                    old = cur;
                }

                // Only add window if we got to document (e.g., not plain obj or detached DOM)
                if (old && old === elem.ownerDocument) {
                    eventPath.push([old.defaultView || old.parentWindow || window, bubbleType]);
                }
            }

            // Fire handlers on the event path
            for (i = 0; i < eventPath.length && !event.isPropagationStopped(); i++) {

                cur = eventPath[i][0];
                event.type = eventPath[i][1];

                handle = (jQuery._data(cur, "events") || {})[event.type] && jQuery._data(cur, "handle");
                if (handle) {
                    handle.apply(cur, data);
                }
                // Note that this is a bare JS function and not a jQuery handler
                handle = ontype && cur[ontype];
                if (handle && jQuery.acceptData(cur) && handle.apply(cur, data) === false) {
                    event.preventDefault();
                }
            }
            event.type = type;

            // If nobody prevented the default action, do it now
            if (!onlyHandlers && !event.isDefaultPrevented()) {

                if ((!special._default || special._default.apply(elem.ownerDocument, data) === false) &&
				!(type === "click" && jQuery.nodeName(elem, "a")) && jQuery.acceptData(elem)) {

                    // Call a native DOM method on the target with the same name name as the event.
                    // Can't use an .isFunction() check here because IE6/7 fails that test.
                    // Don't do default actions on window, that's where global variables be (#6170)
                    // IE<9 dies on focus/blur to hidden element (#1486)
                    if (ontype && elem[type] && ((type !== "focus" && type !== "blur") || event.target.offsetWidth !== 0) && !jQuery.isWindow(elem)) {

                        // Don't re-trigger an onFOO event when we call its FOO() method
                        old = elem[ontype];

                        if (old) {
                            elem[ontype] = null;
                        }

                        // Prevent re-triggering of the same event, since we already bubbled it above
                        jQuery.event.triggered = type;
                        elem[type]();
                        jQuery.event.triggered = undefined;

                        if (old) {
                            elem[ontype] = old;
                        }
                    }
                }
            }

            return event.result;
        },

        dispatch: function (event) {

            // Make a writable jQuery.Event from the native event object
            event = jQuery.event.fix(event || window.event);

            var handlers = ((jQuery._data(this, "events") || {})[event.type] || []),
			delegateCount = handlers.delegateCount,
			args = [].slice.call(arguments, 0),
			run_all = !event.exclusive && !event.namespace,
			handlerQueue = [],
			i, j, cur, jqcur, ret, selMatch, matched, matches, handleObj, sel, related;

            // Use the fix-ed jQuery.Event rather than the (read-only) native event
            args[0] = event;
            event.delegateTarget = this;

            // Determine handlers that should run if there are delegated events
            // Avoid disabled elements in IE (#6911) and non-left-click bubbling in Firefox (#3861)
            if (delegateCount && !event.target.disabled && !(event.button && event.type === "click")) {

                // Pregenerate a single jQuery object for reuse with .is()
                jqcur = jQuery(this);
                jqcur.context = this.ownerDocument || this;

                for (cur = event.target; cur != this; cur = cur.parentNode || this) {
                    selMatch = {};
                    matches = [];
                    jqcur[0] = cur;
                    for (i = 0; i < delegateCount; i++) {
                        handleObj = handlers[i];
                        sel = handleObj.selector;

                        if (selMatch[sel] === undefined) {
                            selMatch[sel] = (
							handleObj.quick ? quickIs(cur, handleObj.quick) : jqcur.is(sel)
						);
                        }
                        if (selMatch[sel]) {
                            matches.push(handleObj);
                        }
                    }
                    if (matches.length) {
                        handlerQueue.push({ elem: cur, matches: matches });
                    }
                }
            }

            // Add the remaining (directly-bound) handlers
            if (handlers.length > delegateCount) {
                handlerQueue.push({ elem: this, matches: handlers.slice(delegateCount) });
            }

            // Run delegates first; they may want to stop propagation beneath us
            for (i = 0; i < handlerQueue.length && !event.isPropagationStopped(); i++) {
                matched = handlerQueue[i];
                event.currentTarget = matched.elem;

                for (j = 0; j < matched.matches.length && !event.isImmediatePropagationStopped(); j++) {
                    handleObj = matched.matches[j];

                    // Triggered event must either 1) be non-exclusive and have no namespace, or
                    // 2) have namespace(s) a subset or equal to those in the bound event (both can have no namespace).
                    if (run_all || (!event.namespace && !handleObj.namespace) || event.namespace_re && event.namespace_re.test(handleObj.namespace)) {

                        event.data = handleObj.data;
                        event.handleObj = handleObj;

                        ret = ((jQuery.event.special[handleObj.origType] || {}).handle || handleObj.handler)
							.apply(matched.elem, args);

                        if (ret !== undefined) {
                            event.result = ret;
                            if (ret === false) {
                                event.preventDefault();
                                event.stopPropagation();
                            }
                        }
                    }
                }
            }

            return event.result;
        },

        // Includes some event props shared by KeyEvent and MouseEvent
        // *** attrChange attrName relatedNode srcElement  are not normalized, non-W3C, deprecated, will be removed in 1.8 ***
        props: "attrChange attrName relatedNode srcElement altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),

        fixHooks: {},

        keyHooks: {
            props: "char charCode key keyCode".split(" "),
            filter: function (event, original) {

                // Add which for key events
                if (event.which == null) {
                    event.which = original.charCode != null ? original.charCode : original.keyCode;
                }

                return event;
            }
        },

        mouseHooks: {
            props: "button buttons clientX clientY fromElement offsetX offsetY pageX pageY screenX screenY toElement".split(" "),
            filter: function (event, original) {
                var eventDoc, doc, body,
				button = original.button,
				fromElement = original.fromElement;

                // Calculate pageX/Y if missing and clientX/Y available
                if (event.pageX == null && original.clientX != null) {
                    eventDoc = event.target.ownerDocument || document;
                    doc = eventDoc.documentElement;
                    body = eventDoc.body;

                    event.pageX = original.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc && doc.clientLeft || body && body.clientLeft || 0);
                    event.pageY = original.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc && doc.clientTop || body && body.clientTop || 0);
                }

                // Add relatedTarget, if necessary
                if (!event.relatedTarget && fromElement) {
                    event.relatedTarget = fromElement === event.target ? original.toElement : fromElement;
                }

                // Add which for click: 1 === left; 2 === middle; 3 === right
                // Note: button is not normalized, so don't use it
                if (!event.which && button !== undefined) {
                    event.which = (button & 1 ? 1 : (button & 2 ? 3 : (button & 4 ? 2 : 0)));
                }

                return event;
            }
        },

        fix: function (event) {
            if (event[jQuery.expando]) {
                return event;
            }

            // Create a writable copy of the event object and normalize some properties
            var i, prop,
			originalEvent = event,
			fixHook = jQuery.event.fixHooks[event.type] || {},
			copy = fixHook.props ? this.props.concat(fixHook.props) : this.props;

            event = jQuery.Event(originalEvent);

            for (i = copy.length; i; ) {
                prop = copy[--i];
                event[prop] = originalEvent[prop];
            }

            // Fix target property, if necessary (#1925, IE 6/7/8 & Safari2)
            if (!event.target) {
                event.target = originalEvent.srcElement || document;
            }

            // Target should not be a text node (#504, Safari)
            if (event.target.nodeType === 3) {
                event.target = event.target.parentNode;
            }

            // For mouse/key events; add metaKey if it's not there (#3368, IE6/7/8)
            if (event.metaKey === undefined) {
                event.metaKey = event.ctrlKey;
            }

            return fixHook.filter ? fixHook.filter(event, originalEvent) : event;
        },

        special: {
            ready: {
                // Make sure the ready event is setup
                setup: jQuery.bindReady
            },

            load: {
                // Prevent triggered image.load events from bubbling to window.load
                noBubble: true
            },

            focus: {
                delegateType: "focusin"
            },
            blur: {
                delegateType: "focusout"
            },

            beforeunload: {
                setup: function (data, namespaces, eventHandle) {
                    // We only want to do this special case on windows
                    if (jQuery.isWindow(this)) {
                        this.onbeforeunload = eventHandle;
                    }
                },

                teardown: function (namespaces, eventHandle) {
                    if (this.onbeforeunload === eventHandle) {
                        this.onbeforeunload = null;
                    }
                }
            }
        },

        simulate: function (type, elem, event, bubble) {
            // Piggyback on a donor event to simulate a different one.
            // Fake originalEvent to avoid donor's stopPropagation, but if the
            // simulated event prevents default then we do the same on the donor.
            var e = jQuery.extend(
			new jQuery.Event(),
			event,
			{ type: type,
			    isSimulated: true,
			    originalEvent: {}
			}
		);
            if (bubble) {
                jQuery.event.trigger(e, null, elem);
            } else {
                jQuery.event.dispatch.call(elem, e);
            }
            if (e.isDefaultPrevented()) {
                event.preventDefault();
            }
        }
    };

    // Some plugins are using, but it's undocumented/deprecated and will be removed.
    // The 1.7 special event interface should provide all the hooks needed now.
    jQuery.event.handle = jQuery.event.dispatch;

    jQuery.removeEvent = document.removeEventListener ?
	function (elem, type, handle) {
	    if (elem.removeEventListener) {
	        elem.removeEventListener(type, handle, false);
	    }
	} :
	function (elem, type, handle) {
	    if (elem.detachEvent) {
	        elem.detachEvent("on" + type, handle);
	    }
	};

    jQuery.Event = function (src, props) {
        // Allow instantiation without the 'new' keyword
        if (!(this instanceof jQuery.Event)) {
            return new jQuery.Event(src, props);
        }

        // Event object
        if (src && src.type) {
            this.originalEvent = src;
            this.type = src.type;

            // Events bubbling up the document may have been marked as prevented
            // by a handler lower down the tree; reflect the correct value.
            this.isDefaultPrevented = (src.defaultPrevented || src.returnValue === false ||
			src.getPreventDefault && src.getPreventDefault()) ? returnTrue : returnFalse;

            // Event type
        } else {
            this.type = src;
        }

        // Put explicitly provided properties onto the event object
        if (props) {
            jQuery.extend(this, props);
        }

        // Create a timestamp if incoming event doesn't have one
        this.timeStamp = src && src.timeStamp || jQuery.now();

        // Mark it as fixed
        this[jQuery.expando] = true;
    };

    function returnFalse() {
        return false;
    }
    function returnTrue() {
        return true;
    }

    // jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
    // http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
    jQuery.Event.prototype = {
        preventDefault: function () {
            this.isDefaultPrevented = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }

            // if preventDefault exists run it on the original event
            if (e.preventDefault) {
                e.preventDefault();

                // otherwise set the returnValue property of the original event to false (IE)
            } else {
                e.returnValue = false;
            }
        },
        stopPropagation: function () {
            this.isPropagationStopped = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }
            // if stopPropagation exists run it on the original event
            if (e.stopPropagation) {
                e.stopPropagation();
            }
            // otherwise set the cancelBubble property of the original event to true (IE)
            e.cancelBubble = true;
        },
        stopImmediatePropagation: function () {
            this.isImmediatePropagationStopped = returnTrue;
            this.stopPropagation();
        },
        isDefaultPrevented: returnFalse,
        isPropagationStopped: returnFalse,
        isImmediatePropagationStopped: returnFalse
    };

    // Create mouseenter/leave events using mouseover/out and event-time checks
    jQuery.each({
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    }, function (orig, fix) {
        jQuery.event.special[orig] = {
            delegateType: fix,
            bindType: fix,

            handle: function (event) {
                var target = this,
				related = event.relatedTarget,
				handleObj = event.handleObj,
				selector = handleObj.selector,
				ret;

                // For mousenter/leave call the handler if related is outside the target.
                // NB: No relatedTarget if the mouse left/entered the browser window
                if (!related || (related !== target && !jQuery.contains(target, related))) {
                    event.type = handleObj.origType;
                    ret = handleObj.handler.apply(this, arguments);
                    event.type = fix;
                }
                return ret;
            }
        };
    });

    // IE submit delegation
    if (!jQuery.support.submitBubbles) {

        jQuery.event.special.submit = {
            setup: function () {
                // Only need this for delegated form submit events
                if (jQuery.nodeName(this, "form")) {
                    return false;
                }

                // Lazy-add a submit handler when a descendant form may potentially be submitted
                jQuery.event.add(this, "click._submit keypress._submit", function (e) {
                    // Node name check avoids a VML-related crash in IE (#9807)
                    var elem = e.target,
					form = jQuery.nodeName(elem, "input") || jQuery.nodeName(elem, "button") ? elem.form : undefined;
                    if (form && !form._submit_attached) {
                        jQuery.event.add(form, "submit._submit", function (event) {
                            // If form was submitted by the user, bubble the event up the tree
                            if (this.parentNode && !event.isTrigger) {
                                jQuery.event.simulate("submit", this.parentNode, event, true);
                            }
                        });
                        form._submit_attached = true;
                    }
                });
                // return undefined since we don't need an event listener
            },

            teardown: function () {
                // Only need this for delegated form submit events
                if (jQuery.nodeName(this, "form")) {
                    return false;
                }

                // Remove delegated handlers; cleanData eventually reaps submit handlers attached above
                jQuery.event.remove(this, "._submit");
            }
        };
    }

    // IE change delegation and checkbox/radio fix
    if (!jQuery.support.changeBubbles) {

        jQuery.event.special.change = {

            setup: function () {

                if (rformElems.test(this.nodeName)) {
                    // IE doesn't fire change on a check/radio until blur; trigger it on click
                    // after a propertychange. Eat the blur-change in special.change.handle.
                    // This still fires onchange a second time for check/radio after blur.
                    if (this.type === "checkbox" || this.type === "radio") {
                        jQuery.event.add(this, "propertychange._change", function (event) {
                            if (event.originalEvent.propertyName === "checked") {
                                this._just_changed = true;
                            }
                        });
                        jQuery.event.add(this, "click._change", function (event) {
                            if (this._just_changed && !event.isTrigger) {
                                this._just_changed = false;
                                jQuery.event.simulate("change", this, event, true);
                            }
                        });
                    }
                    return false;
                }
                // Delegated event; lazy-add a change handler on descendant inputs
                jQuery.event.add(this, "beforeactivate._change", function (e) {
                    var elem = e.target;

                    if (rformElems.test(elem.nodeName) && !elem._change_attached) {
                        jQuery.event.add(elem, "change._change", function (event) {
                            if (this.parentNode && !event.isSimulated && !event.isTrigger) {
                                jQuery.event.simulate("change", this.parentNode, event, true);
                            }
                        });
                        elem._change_attached = true;
                    }
                });
            },

            handle: function (event) {
                var elem = event.target;

                // Swallow native change events from checkbox/radio, we already triggered them above
                if (this !== elem || event.isSimulated || event.isTrigger || (elem.type !== "radio" && elem.type !== "checkbox")) {
                    return event.handleObj.handler.apply(this, arguments);
                }
            },

            teardown: function () {
                jQuery.event.remove(this, "._change");

                return rformElems.test(this.nodeName);
            }
        };
    }

    // Create "bubbling" focus and blur events
    if (!jQuery.support.focusinBubbles) {
        jQuery.each({ focus: "focusin", blur: "focusout" }, function (orig, fix) {

            // Attach a single capturing handler while someone wants focusin/focusout
            var attaches = 0,
			handler = function (event) {
			    jQuery.event.simulate(fix, event.target, jQuery.event.fix(event), true);
			};

            jQuery.event.special[fix] = {
                setup: function () {
                    if (attaches++ === 0) {
                        document.addEventListener(orig, handler, true);
                    }
                },
                teardown: function () {
                    if (--attaches === 0) {
                        document.removeEventListener(orig, handler, true);
                    }
                }
            };
        });
    }

    jQuery.fn.extend({

        on: function (types, selector, data, fn, /*INTERNAL*/one) {
            var origFn, type;

            // Types can be a map of types/handlers
            if (typeof types === "object") {
                // ( types-Object, selector, data )
                if (typeof selector !== "string") {
                    // ( types-Object, data )
                    data = selector;
                    selector = undefined;
                }
                for (type in types) {
                    this.on(type, selector, data, types[type], one);
                }
                return this;
            }

            if (data == null && fn == null) {
                // ( types, fn )
                fn = selector;
                data = selector = undefined;
            } else if (fn == null) {
                if (typeof selector === "string") {
                    // ( types, selector, fn )
                    fn = data;
                    data = undefined;
                } else {
                    // ( types, data, fn )
                    fn = data;
                    data = selector;
                    selector = undefined;
                }
            }
            if (fn === false) {
                fn = returnFalse;
            } else if (!fn) {
                return this;
            }

            if (one === 1) {
                origFn = fn;
                fn = function (event) {
                    // Can use an empty set, since event contains the info
                    jQuery().off(event);
                    return origFn.apply(this, arguments);
                };
                // Use same guid so caller can remove using origFn
                fn.guid = origFn.guid || (origFn.guid = jQuery.guid++);
            }
            return this.each(function () {
                jQuery.event.add(this, types, fn, data, selector);
            });
        },
        one: function (types, selector, data, fn) {
            return this.on.call(this, types, selector, data, fn, 1);
        },
        off: function (types, selector, fn) {
            if (types && types.preventDefault && types.handleObj) {
                // ( event )  dispatched jQuery.Event
                var handleObj = types.handleObj;
                jQuery(types.delegateTarget).off(
				handleObj.namespace ? handleObj.type + "." + handleObj.namespace : handleObj.type,
				handleObj.selector,
				handleObj.handler
			);
                return this;
            }
            if (typeof types === "object") {
                // ( types-object [, selector] )
                for (var type in types) {
                    this.off(type, selector, types[type]);
                }
                return this;
            }
            if (selector === false || typeof selector === "function") {
                // ( types [, fn] )
                fn = selector;
                selector = undefined;
            }
            if (fn === false) {
                fn = returnFalse;
            }
            return this.each(function () {
                jQuery.event.remove(this, types, fn, selector);
            });
        },

        bind: function (types, data, fn) {
            return this.on(types, null, data, fn);
        },
        unbind: function (types, fn) {
            return this.off(types, null, fn);
        },

        live: function (types, data, fn) {
            jQuery(this.context).on(types, this.selector, data, fn);
            return this;
        },
        die: function (types, fn) {
            jQuery(this.context).off(types, this.selector || "**", fn);
            return this;
        },

        delegate: function (selector, types, data, fn) {
            return this.on(types, selector, data, fn);
        },
        undelegate: function (selector, types, fn) {
            // ( namespace ) or ( selector, types [, fn] )
            return arguments.length == 1 ? this.off(selector, "**") : this.off(types, selector, fn);
        },

        trigger: function (type, data) {
            return this.each(function () {
                jQuery.event.trigger(type, data, this);
            });
        },
        triggerHandler: function (type, data) {
            if (this[0]) {
                return jQuery.event.trigger(type, data, this[0], true);
            }
        },

        toggle: function (fn) {
            // Save reference to arguments for access in closure
            var args = arguments,
			guid = fn.guid || jQuery.guid++,
			i = 0,
			toggler = function (event) {
			    // Figure out which function to execute
			    var lastToggle = (jQuery._data(this, "lastToggle" + fn.guid) || 0) % i;
			    jQuery._data(this, "lastToggle" + fn.guid, lastToggle + 1);

			    // Make sure that clicks stop
			    event.preventDefault();

			    // and execute the function
			    return args[lastToggle].apply(this, arguments) || false;
			};

            // link all the functions, so any of them can unbind this click handler
            toggler.guid = guid;
            while (i < args.length) {
                args[i++].guid = guid;
            }

            return this.click(toggler);
        },

        hover: function (fnOver, fnOut) {
            return this.mouseenter(fnOver).mouseleave(fnOut || fnOver);
        }
    });

    jQuery.each(("blur focus focusin focusout load resize scroll unload click dblclick " +
	"mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " +
	"change select submit keydown keypress keyup error contextmenu").split(" "), function (i, name) {

	    // Handle event binding
	    jQuery.fn[name] = function (data, fn) {
	        if (fn == null) {
	            fn = data;
	            data = null;
	        }

	        return arguments.length > 0 ?
			this.on(name, null, data, fn) :
			this.trigger(name);
	    };

	    if (jQuery.attrFn) {
	        jQuery.attrFn[name] = true;
	    }

	    if (rkeyEvent.test(name)) {
	        jQuery.event.fixHooks[name] = jQuery.event.keyHooks;
	    }

	    if (rmouseEvent.test(name)) {
	        jQuery.event.fixHooks[name] = jQuery.event.mouseHooks;
	    }
	});



    /*!
    * Sizzle CSS Selector Engine
    *  Copyright 2011, The Dojo Foundation
    *  Released under the MIT, BSD, and GPL Licenses.
    *  More information: http://sizzlejs.com/
    */
    (function () {

        var chunker = /((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^\[\]]*\]|['"][^'"]*['"]|[^\[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?((?:.|\r|\n)*)/g,
	expando = "sizcache" + (Math.random() + '').replace('.', ''),
	done = 0,
	toString = Object.prototype.toString,
	hasDuplicate = false,
	baseHasDuplicate = true,
	rBackslash = /\\/g,
	rReturn = /\r\n/g,
	rNonWord = /\W/;

        // Here we check if the JavaScript engine is using some sort of
        // optimization where it does not always call our comparision
        // function. If that is the case, discard the hasDuplicate value.
        //   Thus far that includes Google Chrome.
        [0, 0].sort(function () {
            baseHasDuplicate = false;
            return 0;
        });

        var Sizzle = function (selector, context, results, seed) {
            results = results || [];
            context = context || document;

            var origContext = context;

            if (context.nodeType !== 1 && context.nodeType !== 9) {
                return [];
            }

            if (!selector || typeof selector !== "string") {
                return results;
            }

            var m, set, checkSet, extra, ret, cur, pop, i,
		prune = true,
		contextXML = Sizzle.isXML(context),
		parts = [],
		soFar = selector;

            // Reset the position of the chunker regexp (start from head)
            do {
                chunker.exec("");
                m = chunker.exec(soFar);

                if (m) {
                    soFar = m[3];

                    parts.push(m[1]);

                    if (m[2]) {
                        extra = m[3];
                        break;
                    }
                }
            } while (m);

            if (parts.length > 1 && origPOS.exec(selector)) {

                if (parts.length === 2 && Expr.relative[parts[0]]) {
                    set = posProcess(parts[0] + parts[1], context, seed);

                } else {
                    set = Expr.relative[parts[0]] ?
				[context] :
				Sizzle(parts.shift(), context);

                    while (parts.length) {
                        selector = parts.shift();

                        if (Expr.relative[selector]) {
                            selector += parts.shift();
                        }

                        set = posProcess(selector, set, seed);
                    }
                }

            } else {
                // Take a shortcut and set the context if the root selector is an ID
                // (but not if it'll be faster if the inner selector is an ID)
                if (!seed && parts.length > 1 && context.nodeType === 9 && !contextXML &&
				Expr.match.ID.test(parts[0]) && !Expr.match.ID.test(parts[parts.length - 1])) {

                    ret = Sizzle.find(parts.shift(), context, contextXML);
                    context = ret.expr ?
				Sizzle.filter(ret.expr, ret.set)[0] :
				ret.set[0];
                }

                if (context) {
                    ret = seed ?
				{ expr: parts.pop(), set: makeArray(seed)} :
				Sizzle.find(parts.pop(), parts.length === 1 && (parts[0] === "~" || parts[0] === "+") && context.parentNode ? context.parentNode : context, contextXML);

                    set = ret.expr ?
				Sizzle.filter(ret.expr, ret.set) :
				ret.set;

                    if (parts.length > 0) {
                        checkSet = makeArray(set);

                    } else {
                        prune = false;
                    }

                    while (parts.length) {
                        cur = parts.pop();
                        pop = cur;

                        if (!Expr.relative[cur]) {
                            cur = "";
                        } else {
                            pop = parts.pop();
                        }

                        if (pop == null) {
                            pop = context;
                        }

                        Expr.relative[cur](checkSet, pop, contextXML);
                    }

                } else {
                    checkSet = parts = [];
                }
            }

            if (!checkSet) {
                checkSet = set;
            }

            if (!checkSet) {
                Sizzle.error(cur || selector);
            }

            if (toString.call(checkSet) === "[object Array]") {
                if (!prune) {
                    results.push.apply(results, checkSet);

                } else if (context && context.nodeType === 1) {
                    for (i = 0; checkSet[i] != null; i++) {
                        if (checkSet[i] && (checkSet[i] === true || checkSet[i].nodeType === 1 && Sizzle.contains(context, checkSet[i]))) {
                            results.push(set[i]);
                        }
                    }

                } else {
                    for (i = 0; checkSet[i] != null; i++) {
                        if (checkSet[i] && checkSet[i].nodeType === 1) {
                            results.push(set[i]);
                        }
                    }
                }

            } else {
                makeArray(checkSet, results);
            }

            if (extra) {
                Sizzle(extra, origContext, results, seed);
                Sizzle.uniqueSort(results);
            }

            return results;
        };

        Sizzle.uniqueSort = function (results) {
            if (sortOrder) {
                hasDuplicate = baseHasDuplicate;
                results.sort(sortOrder);

                if (hasDuplicate) {
                    for (var i = 1; i < results.length; i++) {
                        if (results[i] === results[i - 1]) {
                            results.splice(i--, 1);
                        }
                    }
                }
            }

            return results;
        };

        Sizzle.matches = function (expr, set) {
            return Sizzle(expr, null, null, set);
        };

        Sizzle.matchesSelector = function (node, expr) {
            return Sizzle(expr, null, null, [node]).length > 0;
        };

        Sizzle.find = function (expr, context, isXML) {
            var set, i, len, match, type, left;

            if (!expr) {
                return [];
            }

            for (i = 0, len = Expr.order.length; i < len; i++) {
                type = Expr.order[i];

                if ((match = Expr.leftMatch[type].exec(expr))) {
                    left = match[1];
                    match.splice(1, 1);

                    if (left.substr(left.length - 1) !== "\\") {
                        match[1] = (match[1] || "").replace(rBackslash, "");
                        set = Expr.find[type](match, context, isXML);

                        if (set != null) {
                            expr = expr.replace(Expr.match[type], "");
                            break;
                        }
                    }
                }
            }

            if (!set) {
                set = typeof context.getElementsByTagName !== "undefined" ?
			context.getElementsByTagName("*") :
			[];
            }

            return { set: set, expr: expr };
        };

        Sizzle.filter = function (expr, set, inplace, not) {
            var match, anyFound,
		type, found, item, filter, left,
		i, pass,
		old = expr,
		result = [],
		curLoop = set,
		isXMLFilter = set && set[0] && Sizzle.isXML(set[0]);

            while (expr && set.length) {
                for (type in Expr.filter) {
                    if ((match = Expr.leftMatch[type].exec(expr)) != null && match[2]) {
                        filter = Expr.filter[type];
                        left = match[1];

                        anyFound = false;

                        match.splice(1, 1);

                        if (left.substr(left.length - 1) === "\\") {
                            continue;
                        }

                        if (curLoop === result) {
                            result = [];
                        }

                        if (Expr.preFilter[type]) {
                            match = Expr.preFilter[type](match, curLoop, inplace, result, not, isXMLFilter);

                            if (!match) {
                                anyFound = found = true;

                            } else if (match === true) {
                                continue;
                            }
                        }

                        if (match) {
                            for (i = 0; (item = curLoop[i]) != null; i++) {
                                if (item) {
                                    found = filter(item, match, i, curLoop);
                                    pass = not ^ found;

                                    if (inplace && found != null) {
                                        if (pass) {
                                            anyFound = true;

                                        } else {
                                            curLoop[i] = false;
                                        }

                                    } else if (pass) {
                                        result.push(item);
                                        anyFound = true;
                                    }
                                }
                            }
                        }

                        if (found !== undefined) {
                            if (!inplace) {
                                curLoop = result;
                            }

                            expr = expr.replace(Expr.match[type], "");

                            if (!anyFound) {
                                return [];
                            }

                            break;
                        }
                    }
                }

                // Improper expression
                if (expr === old) {
                    if (anyFound == null) {
                        Sizzle.error(expr);

                    } else {
                        break;
                    }
                }

                old = expr;
            }

            return curLoop;
        };

        Sizzle.error = function (msg) {
            throw new Error("Syntax error, unrecognized expression: " + msg);
        };

        /**
        * Utility function for retreiving the text value of an array of DOM nodes
        * @param {Array|Element} elem
        */
        var getText = Sizzle.getText = function (elem) {
            var i, node,
		nodeType = elem.nodeType,
		ret = "";

            if (nodeType) {
                if (nodeType === 1 || nodeType === 9) {
                    // Use textContent || innerText for elements
                    if (typeof elem.textContent === 'string') {
                        return elem.textContent;
                    } else if (typeof elem.innerText === 'string') {
                        // Replace IE's carriage returns
                        return elem.innerText.replace(rReturn, '');
                    } else {
                        // Traverse it's children
                        for (elem = elem.firstChild; elem; elem = elem.nextSibling) {
                            ret += getText(elem);
                        }
                    }
                } else if (nodeType === 3 || nodeType === 4) {
                    return elem.nodeValue;
                }
            } else {

                // If no nodeType, this is expected to be an array
                for (i = 0; (node = elem[i]); i++) {
                    // Do not traverse comment nodes
                    if (node.nodeType !== 8) {
                        ret += getText(node);
                    }
                }
            }
            return ret;
        };

        var Expr = Sizzle.selectors = {
            order: ["ID", "NAME", "TAG"],

            match: {
                ID: /#((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
                CLASS: /\.((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
                NAME: /\[name=['"]*((?:[\w\u00c0-\uFFFF\-]|\\.)+)['"]*\]/,
                ATTR: /\[\s*((?:[\w\u00c0-\uFFFF\-]|\\.)+)\s*(?:(\S?=)\s*(?:(['"])(.*?)\3|(#?(?:[\w\u00c0-\uFFFF\-]|\\.)*)|)|)\s*\]/,
                TAG: /^((?:[\w\u00c0-\uFFFF\*\-]|\\.)+)/,
                CHILD: /:(only|nth|last|first)-child(?:\(\s*(even|odd|(?:[+\-]?\d+|(?:[+\-]?\d*)?n\s*(?:[+\-]\s*\d+)?))\s*\))?/,
                POS: /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^\-]|$)/,
                PSEUDO: /:((?:[\w\u00c0-\uFFFF\-]|\\.)+)(?:\((['"]?)((?:\([^\)]+\)|[^\(\)]*)+)\2\))?/
            },

            leftMatch: {},

            attrMap: {
                "class": "className",
                "for": "htmlFor"
            },

            attrHandle: {
                href: function (elem) {
                    return elem.getAttribute("href");
                },
                type: function (elem) {
                    return elem.getAttribute("type");
                }
            },

            relative: {
                "+": function (checkSet, part) {
                    var isPartStr = typeof part === "string",
				isTag = isPartStr && !rNonWord.test(part),
				isPartStrNotTag = isPartStr && !isTag;

                    if (isTag) {
                        part = part.toLowerCase();
                    }

                    for (var i = 0, l = checkSet.length, elem; i < l; i++) {
                        if ((elem = checkSet[i])) {
                            while ((elem = elem.previousSibling) && elem.nodeType !== 1) { }

                            checkSet[i] = isPartStrNotTag || elem && elem.nodeName.toLowerCase() === part ?
						elem || false :
						elem === part;
                        }
                    }

                    if (isPartStrNotTag) {
                        Sizzle.filter(part, checkSet, true);
                    }
                },

                ">": function (checkSet, part) {
                    var elem,
				isPartStr = typeof part === "string",
				i = 0,
				l = checkSet.length;

                    if (isPartStr && !rNonWord.test(part)) {
                        part = part.toLowerCase();

                        for (; i < l; i++) {
                            elem = checkSet[i];

                            if (!jv.IsNull(elem)) {
                                var parent = elem.parentNode;
                                checkSet[i] = parent.nodeName.toLowerCase() === part ? parent : false;
                            }
                        }

                    } else {
                        for (; i < l; i++) {
                            elem = checkSet[i];

                            if (!jv.IsNull(elem)) {
                                checkSet[i] = isPartStr ?
							elem.parentNode :
							elem.parentNode === part;
                            }
                        }

                        if (isPartStr) {
                            Sizzle.filter(part, checkSet, true);
                        }
                    }
                },

                "": function (checkSet, part, isXML) {
                    var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

                    if (typeof part === "string" && !rNonWord.test(part)) {
                        part = part.toLowerCase();
                        nodeCheck = part;
                        checkFn = dirNodeCheck;
                    }

                    checkFn("parentNode", part, doneName, checkSet, nodeCheck, isXML);
                },

                "~": function (checkSet, part, isXML) {
                    var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

                    if (typeof part === "string" && !rNonWord.test(part)) {
                        part = part.toLowerCase();
                        nodeCheck = part;
                        checkFn = dirNodeCheck;
                    }

                    checkFn("previousSibling", part, doneName, checkSet, nodeCheck, isXML);
                }
            },

            find: {
                ID: function (match, context, isXML) {
                    if (typeof context.getElementById !== "undefined" && !isXML) {
                        var m = context.getElementById(match[1]);
                        // Check parentNode to catch when Blackberry 4.6 returns
                        // nodes that are no longer in the document #6963
                        return m && m.parentNode ? [m] : [];
                    }
                },

                NAME: function (match, context) {
                    if (typeof context.getElementsByName !== "undefined") {
                        var ret = [],
					results = context.getElementsByName(match[1]);

                        for (var i = 0, l = results.length; i < l; i++) {
                            if (results[i].getAttribute("name") === match[1]) {
                                ret.push(results[i]);
                            }
                        }

                        return ret.length === 0 ? null : ret;
                    }
                },

                TAG: function (match, context) {
                    if (typeof context.getElementsByTagName !== "undefined") {
                        return context.getElementsByTagName(match[1]);
                    }
                }
            },
            preFilter: {
                CLASS: function (match, curLoop, inplace, result, not, isXML) {
                    match = " " + match[1].replace(rBackslash, "") + " ";

                    if (isXML) {
                        return match;
                    }

                    for (var i = 0, elem; (elem = curLoop[i]) != null; i++) {
                        if (!jv.IsNull(elem)) {
                            if (not ^ (elem.className && (" " + elem.className + " ").replace(/[\t\n\r]/g, " ").indexOf(match) >= 0)) {
                                if (!inplace) {
                                    result.push(elem);
                                }

                            } else if (inplace) {
                                curLoop[i] = false;
                            }
                        }
                    }

                    return false;
                },

                ID: function (match) {
                    return match[1].replace(rBackslash, "");
                },

                TAG: function (match, curLoop) {
                    return match[1].replace(rBackslash, "").toLowerCase();
                },

                CHILD: function (match) {
                    if (match[1] === "nth") {
                        if (!match[2]) {
                            Sizzle.error(match[0]);
                        }

                        match[2] = match[2].replace(/^\+|\s*/g, '');

                        // parse equations like 'even', 'odd', '5', '2n', '3n+2', '4n-1', '-n+6'
                        var test = /(-?)(\d*)(?:n([+\-]?\d*))?/.exec(
					match[2] === "even" && "2n" || match[2] === "odd" && "2n+1" ||
					!/\D/.test(match[2]) && "0n+" + match[2] || match[2]);

                        // calculate the numbers (first)n+(last) including if they are negative
                        match[2] = (test[1] + (test[2] || 1)) - 0;
                        match[3] = test[3] - 0;
                    }
                    else if (match[2]) {
                        Sizzle.error(match[0]);
                    }

                    // TODO: Move to normal caching system
                    match[0] = done++;

                    return match;
                },

                ATTR: function (match, curLoop, inplace, result, not, isXML) {
                    var name = match[1] = match[1].replace(rBackslash, "");

                    if (!isXML && Expr.attrMap[name]) {
                        match[1] = Expr.attrMap[name];
                    }

                    // Handle if an un-quoted value was used
                    match[4] = (match[4] || match[5] || "").replace(rBackslash, "");

                    if (match[2] === "~=") {
                        match[4] = " " + match[4] + " ";
                    }

                    return match;
                },

                PSEUDO: function (match, curLoop, inplace, result, not) {
                    if (match[1] === "not") {
                        // If we're dealing with a complex expression, or a simple one
                        if ((chunker.exec(match[3]) || "").length > 1 || /^\w/.test(match[3])) {
                            match[3] = Sizzle(match[3], null, null, curLoop);

                        } else {
                            var ret = Sizzle.filter(match[3], curLoop, inplace, true ^ not);

                            if (!inplace) {
                                result.push.apply(result, ret);
                            }

                            return false;
                        }

                    } else if (Expr.match.POS.test(match[0]) || Expr.match.CHILD.test(match[0])) {
                        return true;
                    }

                    return match;
                },

                POS: function (match) {
                    match.unshift(true);

                    return match;
                }
            },

            filters: {
                enabled: function (elem) {
                    return elem.disabled === false && elem.type !== "hidden";
                },

                disabled: function (elem) {
                    return elem.disabled === true;
                },

                checked: function (elem) {
                    return elem.checked === true;
                },

                selected: function (elem) {
                    // Accessing this property makes selected-by-default
                    // options in Safari work properly
                    if (elem.parentNode) {
                        elem.parentNode.selectedIndex;
                    }

                    return elem.selected === true;
                },

                parent: function (elem) {
                    return !!elem.firstChild;
                },

                empty: function (elem) {
                    return !elem.firstChild;
                },

                has: function (elem, i, match) {
                    return !!Sizzle(match[3], elem).length;
                },

                header: function (elem) {
                    return (/h\d/i).test(elem.nodeName);
                },

                text: function (elem) {
                    if (jv.IsNull(elem)) return;
                    var attr = elem.getAttribute("type"), type = elem.type;
                    // IE6 and 7 will map elem.type to 'text' for new HTML5 types (search, etc) 
                    // use getAttribute instead to test this case
                    return elem.nodeName.toLowerCase() === "input" && "text" === type && (attr === type || attr === null);
                },

                radio: function (elem) {
                    return elem.nodeName.toLowerCase() === "input" && "radio" === elem.type;
                },

                checkbox: function (elem) {
                    return elem.nodeName.toLowerCase() === "input" && "checkbox" === elem.type;
                },

                file: function (elem) {
                    return elem.nodeName.toLowerCase() === "input" && "file" === elem.type;
                },

                password: function (elem) {
                    return elem.nodeName.toLowerCase() === "input" && "password" === elem.type;
                },

                submit: function (elem) {
                    var name = elem.nodeName.toLowerCase();
                    return (name === "input" || name === "button") && "submit" === elem.type;
                },

                image: function (elem) {
                    return elem.nodeName.toLowerCase() === "input" && "image" === elem.type;
                },

                reset: function (elem) {
                    var name = elem.nodeName.toLowerCase();
                    return (name === "input" || name === "button") && "reset" === elem.type;
                },

                button: function (elem) {
                    var name = elem.nodeName.toLowerCase();
                    return name === "input" && "button" === elem.type || name === "button";
                },

                input: function (elem) {
                    return (/input|select|textarea|button/i).test(elem.nodeName);
                },

                focus: function (elem) {
                    return elem === elem.ownerDocument.activeElement;
                }
            },
            setFilters: {
                first: function (elem, i) {
                    return i === 0;
                },

                last: function (elem, i, match, array) {
                    return i === array.length - 1;
                },

                even: function (elem, i) {
                    return i % 2 === 0;
                },

                odd: function (elem, i) {
                    return i % 2 === 1;
                },

                lt: function (elem, i, match) {
                    return i < match[3] - 0;
                },

                gt: function (elem, i, match) {
                    return i > match[3] - 0;
                },

                nth: function (elem, i, match) {
                    return match[3] - 0 === i;
                },

                eq: function (elem, i, match) {
                    return match[3] - 0 === i;
                }
            },
            filter: {
                PSEUDO: function (elem, match, i, array) {
                    if (jv.IsNull(elem)) return;
                    var name = match[1],
				    filter = Expr.filters[name];

                    if (filter) {
                        return filter(elem, i, match, array);

                    } else if (name === "contains") {
                        return (elem.textContent || elem.innerText || getText([elem]) || "").indexOf(match[3]) >= 0;

                    } else if (name === "not") {
                        var not = match[3];

                        for (var j = 0, l = not.length; j < l; j++) {
                            if (not[j] === elem) {
                                return false;
                            }
                        }

                        return true;

                    } else {
                        Sizzle.error(name);
                    }
                },

                CHILD: function (elem, match) {
                    var first, last,
				doneName, parent, cache,
				count, diff,
				type = match[1],
				node = elem;

                    switch (type) {
                        case "only":
                        case "first":
                            while ((node = node.previousSibling)) {
                                if (node.nodeType === 1) {
                                    return false;
                                }
                            }

                            if (type === "first") {
                                return true;
                            }

                            node = elem;

                        case "last":
                            while ((node = node.nextSibling)) {
                                if (node.nodeType === 1) {
                                    return false;
                                }
                            }

                            return true;

                        case "nth":
                            first = match[2];
                            last = match[3];

                            if (first === 1 && last === 0) {
                                return true;
                            }

                            doneName = match[0];
                            parent = elem.parentNode;

                            if (parent && (parent[expando] !== doneName || !elem.nodeIndex)) {
                                count = 0;

                                for (node = parent.firstChild; node; node = node.nextSibling) {
                                    if (node.nodeType === 1) {
                                        node.nodeIndex = ++count;
                                    }
                                }

                                parent[expando] = doneName;
                            }

                            diff = elem.nodeIndex - last;

                            if (first === 0) {
                                return diff === 0;

                            } else {
                                return (diff % first === 0 && diff / first >= 0);
                            }
                    }
                },

                ID: function (elem, match) {
                    return elem.nodeType === 1 && elem.getAttribute("id") === match;
                },

                TAG: function (elem, match) {
                    return (match === "*" && elem.nodeType === 1) || !!elem.nodeName && elem.nodeName.toLowerCase() === match;
                },

                CLASS: function (elem, match) {
                    return (" " + (elem.className || elem.getAttribute("class")) + " ")
				.indexOf(match) > -1;
                },

                ATTR: function (elem, match) {
                    var name = match[1],
				result = Sizzle.attr ?
					Sizzle.attr(elem, name) :
					Expr.attrHandle[name] ?
					Expr.attrHandle[name](elem) :
					elem[name] != null ?
						elem[name] :
						elem.getAttribute(name),
				value = result + "",
				type = match[2],
				check = match[4];

                    return result == null ?
				type === "!=" :
				!type && Sizzle.attr ?
				result != null :
				type === "=" ?
				value === check :
				type === "*=" ?
				value.indexOf(check) >= 0 :
				type === "~=" ?
				(" " + value + " ").indexOf(check) >= 0 :
				!check ?
				value && result !== false :
				type === "!=" ?
				value !== check :
				type === "^=" ?
				value.indexOf(check) === 0 :
				type === "$=" ?
				value.substr(value.length - check.length) === check :
				type === "|=" ?
				value === check || value.substr(0, check.length + 1) === check + "-" :
				false;
                },

                POS: function (elem, match, i, array) {
                    var name = match[2],
				filter = Expr.setFilters[name];

                    if (filter) {
                        return filter(elem, i, match, array);
                    }
                }
            }
        };

        var origPOS = Expr.match.POS,
	fescape = function (all, num) {
	    return "\\" + (num - 0 + 1);
	};

        for (var type in Expr.match) {
            Expr.match[type] = new RegExp(Expr.match[type].source + (/(?![^\[]*\])(?![^\(]*\))/.source));
            Expr.leftMatch[type] = new RegExp(/(^(?:.|\r|\n)*?)/.source + Expr.match[type].source.replace(/\\(\d+)/g, fescape));
        }

        var makeArray = function (array, results) {
            array = Array.prototype.slice.call(array, 0);

            if (results) {
                results.push.apply(results, array);
                return results;
            }

            return array;
        };

        // Perform a simple check to determine if the browser is capable of
        // converting a NodeList to an array using builtin methods.
        // Also verifies that the returned array holds DOM nodes
        // (which is not the case in the Blackberry browser)
        try {
            Array.prototype.slice.call(document.documentElement.childNodes, 0)[0].nodeType;

            // Provide a fallback method if it does not work
        } catch (e) {
            makeArray = function (array, results) {
                var i = 0,
			ret = results || [];

                if (toString.call(array) === "[object Array]") {
                    Array.prototype.push.apply(ret, array);

                } else {
                    if (typeof array.length === "number") {
                        for (var l = array.length; i < l; i++) {
                            ret.push(array[i]);
                        }

                    } else {
                        for (; array[i]; i++) {
                            ret.push(array[i]);
                        }
                    }
                }

                return ret;
            };
        }

        var sortOrder, siblingCheck;

        if (document.documentElement.compareDocumentPosition) {
            sortOrder = function (a, b) {
                if (a === b) {
                    hasDuplicate = true;
                    return 0;
                }

                if (!a.compareDocumentPosition || !b.compareDocumentPosition) {
                    return a.compareDocumentPosition ? -1 : 1;
                }

                return a.compareDocumentPosition(b) & 4 ? -1 : 1;
            };

        } else {
            sortOrder = function (a, b) {
                // The nodes are identical, we can exit early
                if (a === b) {
                    hasDuplicate = true;
                    return 0;

                    // Fallback to using sourceIndex (in IE) if it's available on both nodes
                } else if (a.sourceIndex && b.sourceIndex) {
                    return a.sourceIndex - b.sourceIndex;
                }

                var al, bl,
			ap = [],
			bp = [],
			aup = a.parentNode,
			bup = b.parentNode,
			cur = aup;

                // If the nodes are siblings (or identical) we can do a quick check
                if (aup === bup) {
                    return siblingCheck(a, b);

                    // If no parents were found then the nodes are disconnected
                } else if (!aup) {
                    return -1;

                } else if (!bup) {
                    return 1;
                }

                // Otherwise they're somewhere else in the tree so we need
                // to build up a full list of the parentNodes for comparison
                while (cur) {
                    ap.unshift(cur);
                    cur = cur.parentNode;
                }

                cur = bup;

                while (cur) {
                    bp.unshift(cur);
                    cur = cur.parentNode;
                }

                al = ap.length;
                bl = bp.length;

                // Start walking down the tree looking for a discrepancy
                for (var i = 0; i < al && i < bl; i++) {
                    if (ap[i] !== bp[i]) {
                        return siblingCheck(ap[i], bp[i]);
                    }
                }

                // We ended someplace up the tree so do a sibling check
                return i === al ?
			siblingCheck(a, bp[i], -1) :
			siblingCheck(ap[i], b, 1);
            };

            siblingCheck = function (a, b, ret) {
                if (a === b) {
                    return ret;
                }

                var cur = a.nextSibling;

                while (cur) {
                    if (cur === b) {
                        return -1;
                    }

                    cur = cur.nextSibling;
                }

                return 1;
            };
        }

        // Check to see if the browser returns elements by name when
        // querying by getElementById (and provide a workaround)
        (function () {
            // We're going to inject a fake input element with a specified name
            var form = document.createElement("div"),
		id = "script" + (new Date()).getTime(),
		root = document.documentElement;

            form.innerHTML = "<a name='" + id + "'/>";

            // Inject it into the root element, check its status, and remove it quickly
            root.insertBefore(form, root.firstChild);

            // The workaround has to do additional checks after a getElementById
            // Which slows things down for other browsers (hence the branching)
            if (document.getElementById(id)) {
                Expr.find.ID = function (match, context, isXML) {
                    if (typeof context.getElementById !== "undefined" && !isXML) {
                        var m = context.getElementById(match[1]);

                        return m ?
					m.id === match[1] || typeof m.getAttributeNode !== "undefined" && m.getAttributeNode("id").nodeValue === match[1] ?
						[m] :
						undefined :
					[];
                    }
                };

                Expr.filter.ID = function (elem, match) {
                    var node = typeof elem.getAttributeNode !== "undefined" && elem.getAttributeNode("id");

                    return elem.nodeType === 1 && node && node.nodeValue === match;
                };
            }

            root.removeChild(form);

            // release memory in IE
            root = form = null;
        })();

        (function () {
            // Check to see if the browser returns only elements
            // when doing getElementsByTagName("*")

            // Create a fake element
            var div = document.createElement("div");
            div.appendChild(document.createComment(""));

            // Make sure no comments are found
            if (div.getElementsByTagName("*").length > 0) {
                Expr.find.TAG = function (match, context) {
                    var results = context.getElementsByTagName(match[1]);

                    // Filter out possible comments
                    if (match[1] === "*") {
                        var tmp = [];

                        for (var i = 0; results[i]; i++) {
                            if (results[i].nodeType === 1) {
                                tmp.push(results[i]);
                            }
                        }

                        results = tmp;
                    }

                    return results;
                };
            }

            // Check to see if an attribute returns normalized href attributes
            div.innerHTML = "<a href='#'></a>";

            if (div.firstChild && typeof div.firstChild.getAttribute !== "undefined" &&
			div.firstChild.getAttribute("href") !== "#") {

                Expr.attrHandle.href = function (elem) {
                    return elem.getAttribute("href", 2);
                };
            }

            // release memory in IE
            div = null;
        })();

        if (document.querySelectorAll) {
            (function () {
                var oldSizzle = Sizzle,
			div = document.createElement("div"),
			id = "__sizzle__";

                div.innerHTML = "<p class='TEST'></p>";

                // Safari can't handle uppercase or unicode characters when
                // in quirks mode.
                if (div.querySelectorAll && div.querySelectorAll(".TEST").length === 0) {
                    return;
                }

                Sizzle = function (query, context, extra, seed) {
                    context = context || document;

                    // Only use querySelectorAll on non-XML documents
                    // (ID selectors don't work in non-HTML documents)
                    if (!seed && !Sizzle.isXML(context)) {
                        // See if we find a selector to speed up
                        var match = /^(\w+$)|^\.([\w\-]+$)|^#([\w\-]+$)/.exec(query);

                        if (match && (context.nodeType === 1 || context.nodeType === 9)) {
                            // Speed-up: Sizzle("TAG")
                            if (match[1]) {
                                return makeArray(context.getElementsByTagName(query), extra);

                                // Speed-up: Sizzle(".CLASS")
                            } else if (match[2] && Expr.find.CLASS && context.getElementsByClassName) {
                                return makeArray(context.getElementsByClassName(match[2]), extra);
                            }
                        }

                        if (context.nodeType === 9) {
                            // Speed-up: Sizzle("body")
                            // The body element only exists once, optimize finding it
                            if (query === "body" && context.body) {
                                return makeArray([context.body], extra);

                                // Speed-up: Sizzle("#ID")
                            } else if (match && match[3]) {
                                var elem = context.getElementById(match[3]);

                                // Check parentNode to catch when Blackberry 4.6 returns
                                // nodes that are no longer in the document #6963
                                if (elem && elem.parentNode) {
                                    // Handle the case where IE and Opera return items
                                    // by name instead of ID
                                    if (elem.id === match[3]) {
                                        return makeArray([elem], extra);
                                    }

                                } else {
                                    return makeArray([], extra);
                                }
                            }

                            try {
                                return makeArray(context.querySelectorAll(query), extra);
                            } catch (qsaError) { }

                            // qSA works strangely on Element-rooted queries
                            // We can work around this by specifying an extra ID on the root
                            // and working up from there (Thanks to Andrew Dupont for the technique)
                            // IE 8 doesn't work on object elements
                        } else if (context.nodeType === 1 && context.nodeName.toLowerCase() !== "object") {
                            var oldContext = context,
						old = context.getAttribute("id"),
						nid = old || id,
						hasParent = context.parentNode,
						relativeHierarchySelector = /^\s*[+~]/.test(query);

                            if (!old) {
                                context.setAttribute("id", nid);
                            } else {
                                nid = nid.replace(/'/g, "\\$&");
                            }
                            if (relativeHierarchySelector && hasParent) {
                                context = context.parentNode;
                            }

                            try {
                                if (!relativeHierarchySelector || hasParent) {
                                    return makeArray(context.querySelectorAll("[id='" + nid + "'] " + query), extra);
                                }

                            } catch (pseudoError) {
                            } finally {
                                if (!old) {
                                    oldContext.removeAttribute("id");
                                }
                            }
                        }
                    }

                    return oldSizzle(query, context, extra, seed);
                };

                for (var prop in oldSizzle) {
                    Sizzle[prop] = oldSizzle[prop];
                }

                // release memory in IE
                div = null;
            })();
        }

        (function () {
            var html = document.documentElement,
		matches = html.matchesSelector || html.mozMatchesSelector || html.webkitMatchesSelector || html.msMatchesSelector;

            if (matches) {
                // Check to see if it's possible to do matchesSelector
                // on a disconnected node (IE 9 fails this)
                var disconnectedMatch = !matches.call(document.createElement("div"), "div"),
			pseudoWorks = false;

                try {
                    // This should fail with an exception
                    // Gecko does not error, returns false instead
                    matches.call(document.documentElement, "[test!='']:sizzle");

                } catch (pseudoError) {
                    pseudoWorks = true;
                }

                Sizzle.matchesSelector = function (node, expr) {
                    // Make sure that attribute selectors are quoted
                    expr = expr.replace(/\=\s*([^'"\]]*)\s*\]/g, "='$1']");

                    if (!Sizzle.isXML(node)) {
                        try {
                            if (pseudoWorks || !Expr.match.PSEUDO.test(expr) && !/!=/.test(expr)) {
                                var ret = matches.call(node, expr);

                                // IE 9's matchesSelector returns false on disconnected nodes
                                if (ret || !disconnectedMatch ||
                                // As well, disconnected nodes are said to be in a document
                                // fragment in IE 9, so check for that
								node.document && node.document.nodeType !== 11) {
                                    return ret;
                                }
                            }
                        } catch (e) { }
                    }

                    return Sizzle(expr, null, null, [node]).length > 0;
                };
            }
        })();

        (function () {
            var div = document.createElement("div");

            div.innerHTML = "<div class='test e'></div><div class='test'></div>";

            // Opera can't find a second classname (in 9.6)
            // Also, make sure that getElementsByClassName actually exists
            if (!div.getElementsByClassName || div.getElementsByClassName("e").length === 0) {
                return;
            }

            // Safari caches class attributes, doesn't catch changes (in 3.2)
            div.lastChild.className = "e";

            if (div.getElementsByClassName("e").length === 1) {
                return;
            }

            Expr.order.splice(1, 0, "CLASS");
            Expr.find.CLASS = function (match, context, isXML) {
                if (typeof context.getElementsByClassName !== "undefined" && !isXML) {
                    return context.getElementsByClassName(match[1]);
                }
            };

            // release memory in IE
            div = null;
        })();

        function dirNodeCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
            for (var i = 0, l = checkSet.length; i < l; i++) {
                var elem = checkSet[i];

                if (!jv.IsNull(elem)) {
                    var match = false;

                    elem = elem[dir];

                    while (elem) {
                        if (elem[expando] === doneName) {
                            match = checkSet[elem.sizset];
                            break;
                        }

                        if (elem.nodeType === 1 && !isXML) {
                            elem[expando] = doneName;
                            elem.sizset = i;
                        }

                        if (elem.nodeName.toLowerCase() === cur) {
                            match = elem;
                            break;
                        }

                        elem = elem[dir];
                    }

                    checkSet[i] = match;
                }
            }
        }

        function dirCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
            for (var i = 0, l = checkSet.length; i < l; i++) {
                var elem = checkSet[i];

                if (!jv.IsNull(elem)) {
                    var match = false;

                    elem = elem[dir];

                    while (elem) {
                        if (elem[expando] === doneName) {
                            match = checkSet[elem.sizset];
                            break;
                        }

                        if (elem.nodeType === 1) {
                            if (!isXML) {
                                elem[expando] = doneName;
                                elem.sizset = i;
                            }

                            if (typeof cur !== "string") {
                                if (elem === cur) {
                                    match = true;
                                    break;
                                }

                            } else if (Sizzle.filter(cur, [elem]).length > 0) {
                                match = elem;
                                break;
                            }
                        }

                        elem = elem[dir];
                    }

                    checkSet[i] = match;
                }
            }
        }

        if (document.documentElement.contains) {
            Sizzle.contains = function (a, b) {
                return a !== b && (a.contains ? a.contains(b) : true);
            };

        } else if (document.documentElement.compareDocumentPosition) {
            Sizzle.contains = function (a, b) {
                return !!(a.compareDocumentPosition(b) & 16);
            };

        } else {
            Sizzle.contains = function () {
                return false;
            };
        }

        Sizzle.isXML = function (elem) {
            // documentElement is verified for cases where it doesn't yet exist
            // (such as loading iframes in IE - #4833) 
            var documentElement = (elem ? elem.ownerDocument || elem : 0).documentElement;

            return documentElement ? documentElement.nodeName !== "HTML" : false;
        };

        var posProcess = function (selector, context, seed) {
            var match,
		tmpSet = [],
		later = "",
		root = context.nodeType ? [context] : context;

            // Position selectors must be done after the filter
            // And so must :not(positional) so we move all PSEUDOs to the end
            while ((match = Expr.match.PSEUDO.exec(selector))) {
                later += match[0];
                selector = selector.replace(Expr.match.PSEUDO, "");
            }

            selector = Expr.relative[selector] ? selector + "*" : selector;

            for (var i = 0, l = root.length; i < l; i++) {
                Sizzle(selector, root[i], tmpSet, seed);
            }

            return Sizzle.filter(later, tmpSet);
        };

        // EXPOSE
        // Override sizzle attribute retrieval
        Sizzle.attr = jQuery.attr;
        Sizzle.selectors.attrMap = {};
        jQuery.find = Sizzle;
        jQuery.expr = Sizzle.selectors;
        jQuery.expr[":"] = jQuery.expr.filters;
        jQuery.unique = Sizzle.uniqueSort;
        jQuery.text = Sizzle.getText;
        jQuery.isXMLDoc = Sizzle.isXML;
        jQuery.contains = Sizzle.contains;


    })();


    var runtil = /Until$/,
	rparentsprev = /^(?:parents|prevUntil|prevAll)/,
    // Note: This RegExp should be improved, or likely pulled from Sizzle
	rmultiselector = /,/,
	isSimple = /^.[^:#\[\.,]*$/,
	slice = Array.prototype.slice,
	POS = jQuery.expr.match.POS,
    // methods guaranteed to produce a unique set when starting from a unique set
	guaranteedUnique = {
	    children: true,
	    contents: true,
	    next: true,
	    prev: true
	};

    jQuery.fn.extend({
        find: function (selector) {
            var self = this,
			i, l;

            if (typeof selector !== "string") {
                return jQuery(selector).filter(function () {
                    for (i = 0, l = self.length; i < l; i++) {
                        if (jQuery.contains(self[i], this)) {
                            return true;
                        }
                    }
                });
            }

            var ret = this.pushStack("", "find", selector),
			length, n, r;

            for (i = 0, l = this.length; i < l; i++) {
                length = ret.length;
                jQuery.find(selector, this[i], ret);

                if (i > 0) {
                    // Make sure that the results are unique
                    for (n = length; n < ret.length; n++) {
                        for (r = 0; r < length; r++) {
                            if (ret[r] === ret[n]) {
                                ret.splice(n--, 1);
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        },

        has: function (target) {
            var targets = jQuery(target);
            return this.filter(function () {
                for (var i = 0, l = targets.length; i < l; i++) {
                    if (jQuery.contains(this, targets[i])) {
                        return true;
                    }
                }
            });
        },

        not: function (selector) {
            return this.pushStack(winnow(this, selector, false), "not", selector);
        },

        filter: function (selector) {
            return this.pushStack(winnow(this, selector, true), "filter", selector);
        },

        is: function (selector) {
            return !!selector && (
			typeof selector === "string" ?
            // If this is a positional selector, check membership in the returned set
            // so $("p:first").is("p:last") won't return true for a doc with two "p".
				POS.test(selector) ?
					jQuery(selector, this.context).index(this[0]) >= 0 :
					jQuery.filter(selector, this).length > 0 :
				this.filter(selector).length > 0);
        },

        closest: function (selectors, context) {
            var ret = [], i, l, cur = this[0];

            // Array (deprecated as of jQuery 1.7)
            if (jQuery.isArray(selectors)) {
                var level = 1;

                while (cur && cur.ownerDocument && cur !== context) {
                    for (i = 0; i < selectors.length; i++) {

                        if (jQuery(cur).is(selectors[i])) {
                            ret.push({ selector: selectors[i], elem: cur, level: level });
                        }
                    }

                    cur = cur.parentNode;
                    level++;
                }

                return ret;
            }

            // String
            var pos = POS.test(selectors) || typeof selectors !== "string" ?
				jQuery(selectors, context || this.context) :
				0;

            for (i = 0, l = this.length; i < l; i++) {
                cur = this[i];

                while (cur) {
                    if (pos ? pos.index(cur) > -1 : jQuery.find.matchesSelector(cur, selectors)) {
                        ret.push(cur);
                        break;

                    } else {
                        cur = cur.parentNode;
                        if (!cur || !cur.ownerDocument || cur === context || cur.nodeType === 11) {
                            break;
                        }
                    }
                }
            }

            ret = ret.length > 1 ? jQuery.unique(ret) : ret;

            return this.pushStack(ret, "closest", selectors);
        },

        // Determine the position of an element within
        // the matched set of elements
        index: function (elem) {

            // No argument, return index in parent
            if (!elem) {
                return (this[0] && this[0].parentNode) ? this.prevAll().length : -1;
            }

            // index in selector
            if (typeof elem === "string") {
                return jQuery.inArray(this[0], jQuery(elem));
            }

            // Locate the position of the desired element
            return jQuery.inArray(
            // If it receives a jQuery object, the first element is used
			elem.jquery ? elem[0] : elem, this);
        },

        add: function (selector, context) {
            var set = typeof selector === "string" ?
				jQuery(selector, context) :
				jQuery.makeArray(selector && selector.nodeType ? [selector] : selector),
			all = jQuery.merge(this.get(), set);

            return this.pushStack(isDisconnected(set[0]) || isDisconnected(all[0]) ?
			all :
			jQuery.unique(all));
        },

        andSelf: function () {
            return this.add(this.prevObject);
        }
    });

    // A painfully simple check to see if an element is disconnected
    // from a document (should be improved, where feasible).
    function isDisconnected(node) {
        return !node || !node.parentNode || node.parentNode.nodeType === 11;
    }

    jQuery.each({
        parent: function (elem) {
            var parent = elem.parentNode;
            return parent && parent.nodeType !== 11 ? parent : null;
        },
        parents: function (elem) {
            return jQuery.dir(elem, "parentNode");
        },
        parentsUntil: function (elem, i, until) {
            return jQuery.dir(elem, "parentNode", until);
        },
        next: function (elem) {
            return jQuery.nth(elem, 2, "nextSibling");
        },
        prev: function (elem) {
            return jQuery.nth(elem, 2, "previousSibling");
        },
        nextAll: function (elem) {
            return jQuery.dir(elem, "nextSibling");
        },
        prevAll: function (elem) {
            return jQuery.dir(elem, "previousSibling");
        },
        nextUntil: function (elem, i, until) {
            return jQuery.dir(elem, "nextSibling", until);
        },
        prevUntil: function (elem, i, until) {
            return jQuery.dir(elem, "previousSibling", until);
        },
        siblings: function (elem) {
            return jQuery.sibling(elem.parentNode.firstChild, elem);
        },
        children: function (elem) {
            return jQuery.sibling(elem.firstChild);
        },
        contents: function (elem) {
            return jQuery.nodeName(elem, "iframe") ?
			elem.contentDocument || elem.contentWindow.document :
			jQuery.makeArray(elem.childNodes);
        }
    }, function (name, fn) {
        jQuery.fn[name] = function (until, selector) {
            var ret = jQuery.map(this, fn, until);

            if (!runtil.test(name)) {
                selector = until;
            }

            if (selector && typeof selector === "string") {
                ret = jQuery.filter(selector, ret);
            }

            ret = this.length > 1 && !guaranteedUnique[name] ? jQuery.unique(ret) : ret;

            if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
                ret = ret.reverse();
            }

            return this.pushStack(ret, name, slice.call(arguments).join(","));
        };
    });

    jQuery.extend({
        filter: function (expr, elems, not) {
            if (not) {
                expr = ":not(" + expr + ")";
            }

            return elems.length === 1 ?
			jQuery.find.matchesSelector(elems[0], expr) ? [elems[0]] : [] :
			jQuery.find.matches(expr, elems);
        },

        dir: function (elem, dir, until) {
            var matched = [],
			cur = elem[dir];

            while (cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery(cur).is(until))) {
                if (cur.nodeType === 1) {
                    matched.push(cur);
                }
                cur = cur[dir];
            }
            return matched;
        },

        nth: function (cur, result, dir, elem) {
            result = result || 1;
            var num = 0;

            for (; cur; cur = cur[dir]) {
                if (cur.nodeType === 1 && ++num === result) {
                    break;
                }
            }

            return cur;
        },

        sibling: function (n, elem) {
            var r = [];

            for (; n; n = n.nextSibling) {
                if (n.nodeType === 1 && n !== elem) {
                    r.push(n);
                }
            }

            return r;
        }
    });

    // Implement the identical functionality for filter and not
    function winnow(elements, qualifier, keep) {

        // Can't pass null or undefined to indexOf in Firefox 4
        // Set to 0 to skip string check
        qualifier = qualifier || 0;

        if (jQuery.isFunction(qualifier)) {
            return jQuery.grep(elements, function (elem, i) {
                var retVal = !!qualifier.call(elem, i, elem);
                return retVal === keep;
            });

        } else if (qualifier.nodeType) {
            return jQuery.grep(elements, function (elem, i) {
                return (elem === qualifier) === keep;
            });

        } else if (typeof qualifier === "string") {
            var filtered = jQuery.grep(elements, function (elem) {
                return elem.nodeType === 1;
            });

            if (isSimple.test(qualifier)) {
                return jQuery.filter(qualifier, filtered, !keep);
            } else {
                qualifier = jQuery.filter(qualifier, filtered);
            }
        }

        return jQuery.grep(elements, function (elem, i) {
            return (jQuery.inArray(elem, qualifier) >= 0) === keep;
        });
    }




    function createSafeFragment(document) {
        var list = nodeNames.split("|"),
	safeFrag = document.createDocumentFragment();

        if (safeFrag.createElement) {
            while (list.length) {
                safeFrag.createElement(
				list.pop()
			);
            }
        }
        return safeFrag;
    }

    var nodeNames = "abbr|article|aside|audio|canvas|datalist|details|figcaption|figure|footer|" +
		"header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",
	rinlinejQuery = / jQuery\d+="(?:\d+|null)"/g,
	rleadingWhitespace = /^\s+/,
	rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig,
	rtagName = /<([\w:]+)/,
	rtbody = /<tbody/i,
	rhtml = /<|&#?\w+;/,
	rnoInnerhtml = /<(?:script|style)/i,
	rnocache = /<(?:script|object|embed|option|style)/i,
	rnoshimcache = new RegExp("<(?:" + nodeNames + ")", "i"),
    // checked="checked" or checked
	rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
	rscriptType = /\/(java|ecma)script/i,
	rcleanScript = /^\s*<!(?:\[CDATA\[|\-\-)/,
	wrapMap = {
	    option: [1, "<select multiple='multiple'>", "</select>"],
	    legend: [1, "<fieldset>", "</fieldset>"],
	    thead: [1, "<table>", "</table>"],
	    tr: [2, "<table><tbody>", "</tbody></table>"],
	    td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],
	    col: [2, "<table><tbody></tbody><colgroup>", "</colgroup></table>"],
	    area: [1, "<map>", "</map>"],
	    _default: [0, "", ""]
	},
	safeFragment = createSafeFragment(document);

    wrapMap.optgroup = wrapMap.option;
    wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
    wrapMap.th = wrapMap.td;

    // IE can't serialize <link> and <script> tags normally
    if (!jQuery.support.htmlSerialize) {
        wrapMap._default = [1, "div<div>", "</div>"];
    }

    jQuery.fn.extend({
        text: function (text) {
            if (jQuery.isFunction(text)) {
                return this.each(function (i) {
                    var self = jQuery(this);

                    self.text(text.call(this, i, self.text()));
                });
            }

            if (typeof text !== "object" && text !== undefined) {
                return this.empty().append((this[0] && this[0].ownerDocument || document).createTextNode(text));
            }

            return jQuery.text(this);
        },

        wrapAll: function (html) {
            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapAll(html.call(this, i));
                });
            }

            if (this[0]) {
                // The elements to wrap the target around
                var wrap = jQuery(html, this[0].ownerDocument).eq(0).clone(true);

                if (this[0].parentNode) {
                    wrap.insertBefore(this[0]);
                }

                wrap.map(function () {
                    var elem = this;

                    while (elem.firstChild && elem.firstChild.nodeType === 1) {
                        elem = elem.firstChild;
                    }

                    return elem;
                }).append(this);
            }

            return this;
        },

        wrapInner: function (html) {
            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapInner(html.call(this, i));
                });
            }

            return this.each(function () {
                var self = jQuery(this),
				contents = self.contents();

                if (contents.length) {
                    contents.wrapAll(html);

                } else {
                    self.append(html);
                }
            });
        },

        wrap: function (html) {
            var isFunction = jQuery.isFunction(html);

            return this.each(function (i) {
                jQuery(this).wrapAll(isFunction ? html.call(this, i) : html);
            });
        },

        unwrap: function () {
            return this.parent().each(function () {
                if (!jQuery.nodeName(this, "body")) {
                    jQuery(this).replaceWith(this.childNodes);
                }
            }).end();
        },

        append: function () {
            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1) {
                    this.appendChild(elem);
                }
            });
        },

        prepend: function () {
            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1) {
                    this.insertBefore(elem, this.firstChild);
                }
            });
        },

        before: function () {
            if (this[0] && this[0].parentNode) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this);
                });
            } else if (arguments.length) {
                var set = jQuery.clean(arguments);
                set.push.apply(set, this.toArray());
                return this.pushStack(set, "before", arguments);
            }
        },

        after: function () {
            if (this[0] && this[0].parentNode) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this.nextSibling);
                });
            } else if (arguments.length) {
                var set = this.pushStack(this, "after", arguments);
                set.push.apply(set, jQuery.clean(arguments));
                return set;
            }
        },

        // keepData is for internal use only--do not document
        remove: function (selector, keepData) {
            for (var i = 0, elem; (elem = this[i]) != null; i++) {
                if (!selector || jQuery.filter(selector, [elem]).length) {
                    if (!keepData && elem.nodeType === 1) {
                        jQuery.cleanData(elem.getElementsByTagName("*"));
                        jQuery.cleanData([elem]);
                    }

                    if (elem.parentNode) {
                        elem.parentNode.removeChild(elem);
                    }
                }
            }

            return this;
        },

        empty: function () {
            for (var i = 0, elem; (elem = this[i]) != null; i++) {
                // Remove element nodes and prevent memory leaks
                if (elem.nodeType === 1) {
                    jQuery.cleanData(elem.getElementsByTagName("*"));
                }

                // Remove any remaining nodes
                while (elem.firstChild) {
                    elem.removeChild(elem.firstChild);
                }
            }

            return this;
        },

        clone: function (dataAndEvents, deepDataAndEvents) {
            dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
            deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

            return this.map(function () {
                return jQuery.clone(this, dataAndEvents, deepDataAndEvents);
            });
        },

        html: function (value) {
            if (value === undefined) {
                return this[0] && this[0].nodeType === 1 ?
				this[0].innerHTML.replace(rinlinejQuery, "") :
				null;

                // See if we can take a shortcut and just use innerHTML
            } else if (typeof value === "string" && !rnoInnerhtml.test(value) &&
			(jQuery.support.leadingWhitespace || !rleadingWhitespace.test(value)) &&
			!wrapMap[(rtagName.exec(value) || ["", ""])[1].toLowerCase()]) {

                value = value.replace(rxhtmlTag, "<$1></$2>");

                try {
                    for (var i = 0, l = this.length; i < l; i++) {
                        // Remove element nodes and prevent memory leaks
                        if (this[i].nodeType === 1) {
                            jQuery.cleanData(this[i].getElementsByTagName("*"));
                            this[i].innerHTML = value;
                        }
                    }

                    // If using innerHTML throws an exception, use the fallback method
                } catch (e) {
                    this.empty().append(value);
                }

            } else if (jQuery.isFunction(value)) {
                this.each(function (i) {
                    var self = jQuery(this);

                    self.html(value.call(this, i, self.html()));
                });

            } else {
                this.empty().append(value);
            }

            return this;
        },

        replaceWith: function (value) {
            if (this[0] && this[0].parentNode) {
                // Make sure that the elements are removed from the DOM before they are inserted
                // this can help fix replacing a parent with child elements
                if (jQuery.isFunction(value)) {
                    return this.each(function (i) {
                        var self = jQuery(this), old = self.html();
                        self.replaceWith(value.call(this, i, old));
                    });
                }

                if (typeof value !== "string") {
                    value = jQuery(value).detach();
                }

                return this.each(function () {
                    var next = this.nextSibling,
					parent = this.parentNode;

                    jQuery(this).remove();

                    if (next) {
                        jQuery(next).before(value);
                    } else {
                        jQuery(parent).append(value);
                    }
                });
            } else {
                return this.length ?
				this.pushStack(jQuery(jQuery.isFunction(value) ? value() : value), "replaceWith", value) :
				this;
            }
        },

        detach: function (selector) {
            return this.remove(selector, true);
        },

        domManip: function (args, table, callback) {
            var results, first, fragment, parent,
			value = args[0],
			scripts = [];

            // We can't cloneNode fragments that contain checked, in WebKit
            if (!jQuery.support.checkClone && arguments.length === 3 && typeof value === "string" && rchecked.test(value)) {
                return this.each(function () {
                    jQuery(this).domManip(args, table, callback, true);
                });
            }

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    args[0] = value.call(this, i, table ? self.html() : undefined);
                    self.domManip(args, table, callback);
                });
            }

            if (this[0]) {
                parent = value && value.parentNode;

                // If we're in a fragment, just use that instead of building a new one
                if (jQuery.support.parentNode && parent && parent.nodeType === 11 && parent.childNodes.length === this.length) {
                    results = { fragment: parent };

                } else {
                    results = jQuery.buildFragment(args, this, scripts);
                }

                fragment = results.fragment;

                if (fragment.childNodes.length === 1) {
                    first = fragment = fragment.firstChild;
                } else {
                    first = fragment.firstChild;
                }

                if (first) {
                    table = table && jQuery.nodeName(first, "tr");

                    for (var i = 0, l = this.length, lastIndex = l - 1; i < l; i++) {
                        callback.call(
						table ?
							root(this[i], first) :
							this[i],
                        // Make sure that we do not leak memory by inadvertently discarding
                        // the original fragment (which might have attached data) instead of
                        // using it; in addition, use the original fragment object for the last
                        // item instead of first because it can end up being emptied incorrectly
                        // in certain situations (Bug #8070).
                        // Fragments from the fragment cache must always be cloned and never used
                        // in place.
						results.cacheable || (l > 1 && i < lastIndex) ?
							jQuery.clone(fragment, true, true) :
							fragment
					);
                    }
                }

                if (scripts.length) {
                    jQuery.each(scripts, evalScript);
                }
            }

            return this;
        }
    });

    function root(elem, cur) {
        return jQuery.nodeName(elem, "table") ?
		(elem.getElementsByTagName("tbody")[0] ||
		elem.appendChild(elem.ownerDocument.createElement("tbody"))) :
		elem;
    }

    function cloneCopyEvent(src, dest) {

        if (dest.nodeType !== 1 || !jQuery.hasData(src)) {
            return;
        }

        var type, i, l,
		oldData = jQuery._data(src),
		curData = jQuery._data(dest, oldData),
		events = oldData.events;

        if (events) {
            delete curData.handle;
            curData.events = {};

            for (type in events) {
                for (i = 0, l = events[type].length; i < l; i++) {
                    jQuery.event.add(dest, type + (events[type][i].namespace ? "." : "") + events[type][i].namespace, events[type][i], events[type][i].data);
                }
            }
        }

        // make the cloned public data object a copy from the original
        if (curData.data) {
            curData.data = jQuery.extend({}, curData.data);
        }
    }

    function cloneFixAttributes(src, dest) {
        var nodeName;

        // We do not need to do anything for non-Elements
        if (dest.nodeType !== 1) {
            return;
        }

        // clearAttributes removes the attributes, which we don't want,
        // but also removes the attachEvent events, which we *do* want
        if (dest.clearAttributes) {
            dest.clearAttributes();
        }

        // mergeAttributes, in contrast, only merges back on the
        // original attributes, not the events
        if (dest.mergeAttributes) {
            dest.mergeAttributes(src);
        }

        nodeName = dest.nodeName.toLowerCase();

        // IE6-8 fail to clone children inside object elements that use
        // the proprietary classid attribute value (rather than the type
        // attribute) to identify the type of content to display
        if (nodeName === "object") {
            dest.outerHTML = src.outerHTML;

        } else if (nodeName === "input" && (src.type === "checkbox" || src.type === "radio")) {
            // IE6-8 fails to persist the checked state of a cloned checkbox
            // or radio button. Worse, IE6-7 fail to give the cloned element
            // a checked appearance if the defaultChecked value isn't also set
            if (src.checked) {
                dest.defaultChecked = dest.checked = src.checked;
            }

            // IE6-7 get confused and end up setting the value of a cloned
            // checkbox/radio button to an empty string instead of "on"
            if (dest.value !== src.value) {
                dest.value = src.value;
            }

            // IE6-8 fails to return the selected option to the default selected
            // state when cloning options
        } else if (nodeName === "option") {
            dest.selected = src.defaultSelected;

            // IE6-8 fails to set the defaultValue to the correct value when
            // cloning other types of input fields
        } else if (nodeName === "input" || nodeName === "textarea") {
            dest.defaultValue = src.defaultValue;
        }

        // Event data gets referenced instead of copied if the expando
        // gets copied too
        dest.removeAttribute(jQuery.expando);
    }

    jQuery.buildFragment = function (args, nodes, scripts) {
        var fragment, cacheable, cacheresults, doc,
	first = args[0];

        // nodes may contain either an explicit document object,
        // a jQuery collection or context object.
        // If nodes[0] contains a valid object to assign to doc
        if (nodes && nodes[0]) {
            doc = nodes[0].ownerDocument || nodes[0];
        }

        // Ensure that an attr object doesn't incorrectly stand in as a document object
        // Chrome and Firefox seem to allow this to occur and will throw exception
        // Fixes #8950
        if (!doc.createDocumentFragment) {
            doc = document;
        }

        // Only cache "small" (1/2 KB) HTML strings that are associated with the main document
        // Cloning options loses the selected state, so don't cache them
        // IE 6 doesn't like it when you put <object> or <embed> elements in a fragment
        // Also, WebKit does not clone 'checked' attributes on cloneNode, so don't cache
        // Lastly, IE6,7,8 will not correctly reuse cached fragments that were created from unknown elems #10501
        if (args.length === 1 && typeof first === "string" && first.length < 512 && doc === document &&
		first.charAt(0) === "<" && !rnocache.test(first) &&
		(jQuery.support.checkClone || !rchecked.test(first)) &&
		(jQuery.support.html5Clone || !rnoshimcache.test(first))) {

            cacheable = true;

            cacheresults = jQuery.fragments[first];
            if (cacheresults && cacheresults !== 1) {
                fragment = cacheresults;
            }
        }

        if (!fragment) {
            fragment = doc.createDocumentFragment();
            jQuery.clean(args, doc, fragment, scripts);
        }

        if (cacheable) {
            jQuery.fragments[first] = cacheresults ? fragment : 1;
        }

        return { fragment: fragment, cacheable: cacheable };
    };

    jQuery.fragments = {};

    jQuery.each({
        appendTo: "append",
        prependTo: "prepend",
        insertBefore: "before",
        insertAfter: "after",
        replaceAll: "replaceWith"
    }, function (name, original) {
        jQuery.fn[name] = function (selector) {
            var ret = [],
			insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

            if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
                insert[original](this[0]);
                return this;

            } else {
                for (var i = 0, l = insert.length; i < l; i++) {
                    var elems = (i > 0 ? this.clone(true) : this).get();
                    jQuery(insert[i])[original](elems);
                    ret = ret.concat(elems);
                }

                return this.pushStack(ret, name, insert.selector);
            }
        };
    });

    function getAll(elem) {
        if (typeof elem.getElementsByTagName !== "undefined") {
            return elem.getElementsByTagName("*");

        } else if (typeof elem.querySelectorAll !== "undefined") {
            return elem.querySelectorAll("*");

        } else {
            return [];
        }
    }

    // Used in clean, fixes the defaultChecked property
    function fixDefaultChecked(elem) {
        if (elem.type === "checkbox" || elem.type === "radio") {
            elem.defaultChecked = elem.checked;
        }
    }
    // Finds all inputs and passes them to fixDefaultChecked
    function findInputs(elem) {
        var nodeName = (elem.nodeName || "").toLowerCase();
        if (nodeName === "input") {
            fixDefaultChecked(elem);
            // Skip scripts, get other children
        } else if (nodeName !== "script" && typeof elem.getElementsByTagName !== "undefined") {
            jQuery.grep(elem.getElementsByTagName("input"), fixDefaultChecked);
        }
    }

    // Derived From: http://www.iecss.com/shimprove/javascript/shimprove.1-0-1.js
    function shimCloneNode(elem) {
        var div = document.createElement("div");
        safeFragment.appendChild(div);

        div.innerHTML = elem.outerHTML;
        return div.firstChild;
    }

    jQuery.extend({
        clone: function (elem, dataAndEvents, deepDataAndEvents) {
            var srcElements,
			destElements,
			i,
            // IE<=8 does not properly clone detached, unknown element nodes
			clone = jQuery.support.html5Clone || !rnoshimcache.test("<" + elem.nodeName) ?
				elem.cloneNode(true) :
				shimCloneNode(elem);

            if ((!jQuery.support.noCloneEvent || !jQuery.support.noCloneChecked) &&
				(elem.nodeType === 1 || elem.nodeType === 11) && !jQuery.isXMLDoc(elem)) {
                // IE copies events bound via attachEvent when using cloneNode.
                // Calling detachEvent on the clone will also remove the events
                // from the original. In order to get around this, we use some
                // proprietary methods to clear the events. Thanks to MooTools
                // guys for this hotness.

                cloneFixAttributes(elem, clone);

                // Using Sizzle here is crazy slow, so we use getElementsByTagName instead
                srcElements = getAll(elem);
                destElements = getAll(clone);

                // Weird iteration because IE will replace the length property
                // with an element if you are cloning the body and one of the
                // elements on the page has a name or id of "length"
                for (i = 0; srcElements[i]; ++i) {
                    // Ensure that the destination node is not null; Fixes #9587
                    if (destElements[i]) {
                        cloneFixAttributes(srcElements[i], destElements[i]);
                    }
                }
            }

            // Copy the events from the original to the clone
            if (dataAndEvents) {
                cloneCopyEvent(elem, clone);

                if (deepDataAndEvents) {
                    srcElements = getAll(elem);
                    destElements = getAll(clone);

                    for (i = 0; srcElements[i]; ++i) {
                        cloneCopyEvent(srcElements[i], destElements[i]);
                    }
                }
            }

            srcElements = destElements = null;

            // Return the cloned set
            return clone;
        },

        clean: function (elems, context, fragment, scripts) {
            var checkScriptType;

            context = context || document;

            // !context.createElement fails in IE with an error but returns typeof 'object'
            if (typeof context.createElement === "undefined") {
                context = context.ownerDocument || context[0] && context[0].ownerDocument || document;
            }

            var ret = [], j;

            for (var i = 0, elem; (elem = elems[i]) != null; i++) {
                if (typeof elem === "number") {
                    elem += "";
                }

                if (!elem) {
                    continue;
                }

                // Convert html string into DOM nodes
                if (typeof elem === "string") {
                    if (!rhtml.test(elem)) {
                        elem = context.createTextNode(elem);
                    } else {
                        // Fix "XHTML"-style tags in all browsers
                        elem = elem.replace(rxhtmlTag, "<$1></$2>");

                        // Trim whitespace, otherwise indexOf won't work as expected
                        var tag = (rtagName.exec(elem) || ["", ""])[1].toLowerCase(),
						wrap = wrapMap[tag] || wrapMap._default,
						depth = wrap[0],
						div = context.createElement("div");

                        // Append wrapper element to unknown element safe doc fragment
                        if (context === document) {
                            // Use the fragment we've already created for this document
                            safeFragment.appendChild(div);
                        } else {
                            // Use a fragment created with the owner document
                            createSafeFragment(context).appendChild(div);
                        }

                        // Go to html and back, then peel off extra wrappers
                        div.innerHTML = wrap[1] + elem + wrap[2];

                        // Move to the right depth
                        while (depth--) {
                            div = div.lastChild;
                        }

                        // Remove IE's autoinserted <tbody> from table fragments
                        if (!jQuery.support.tbody) {

                            // String was a <table>, *may* have spurious <tbody>
                            var hasBody = rtbody.test(elem),
							tbody = tag === "table" && !hasBody ?
								div.firstChild && div.firstChild.childNodes :

                            // String was a bare <thead> or <tfoot>
								wrap[1] === "<table>" && !hasBody ?
									div.childNodes :
									[];

                            for (j = tbody.length - 1; j >= 0; --j) {
                                if (jQuery.nodeName(tbody[j], "tbody") && !tbody[j].childNodes.length) {
                                    tbody[j].parentNode.removeChild(tbody[j]);
                                }
                            }
                        }

                        // IE completely kills leading whitespace when innerHTML is used
                        if (!jQuery.support.leadingWhitespace && rleadingWhitespace.test(elem)) {
                            div.insertBefore(context.createTextNode(rleadingWhitespace.exec(elem)[0]), div.firstChild);
                        }

                        elem = div.childNodes;
                    }
                }

                // Resets defaultChecked for any radios and checkboxes
                // about to be appended to the DOM in IE 6/7 (#8060)
                var len;
                if (!jQuery.support.appendChecked) {
                    if (elem[0] && typeof (len = elem.length) === "number") {
                        for (j = 0; j < len; j++) {
                            findInputs(elem[j]);
                        }
                    } else {
                        findInputs(elem);
                    }
                }

                if (elem.nodeType) {
                    ret.push(elem);
                } else {
                    ret = jQuery.merge(ret, elem);
                }
            }

            if (fragment) {
                checkScriptType = function (elem) {
                    return !elem.type || rscriptType.test(elem.type);
                };
                for (i = 0; ret[i]; i++) {
                    if (scripts && jQuery.nodeName(ret[i], "script") && (!ret[i].type || ret[i].type.toLowerCase() === "text/javascript")) {
                        scripts.push(ret[i].parentNode ? ret[i].parentNode.removeChild(ret[i]) : ret[i]);

                    } else {
                        if (ret[i].nodeType === 1) {
                            var jsTags = jQuery.grep(ret[i].getElementsByTagName("script"), checkScriptType);

                            ret.splice.apply(ret, [i + 1, 0].concat(jsTags));
                        }
                        fragment.appendChild(ret[i]);
                    }
                }
            }

            return ret;
        },

        cleanData: function (elems) {
            var data, id,
			cache = jQuery.cache,
			special = jQuery.event.special,
			deleteExpando = jQuery.support.deleteExpando;

            for (var i = 0, elem; (elem = elems[i]) != null; i++) {
                if (jv.IsNull(elem)) continue;
                if (elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()]) {
                    continue;
                }

                id = elem[jQuery.expando];

                if (id) {
                    data = cache[id];

                    if (data && data.events) {
                        for (var type in data.events) {
                            if (special[type]) {
                                jQuery.event.remove(elem, type);

                                // This is a shortcut to avoid jQuery.event.remove's overhead
                            } else {
                                jQuery.removeEvent(elem, type, data.handle);
                            }
                        }

                        // Null the DOM reference to avoid IE6/7/8 leak (#7054)
                        if (data.handle) {
                            data.handle.elem = null;
                        }
                    }

                    if (deleteExpando) {
                        delete elem[jQuery.expando];

                    } else if (elem.removeAttribute) {
                        elem.removeAttribute(jQuery.expando);
                    }

                    delete cache[id];
                }
            }
        }
    });

    function evalScript(i, elem) {
        if (elem.src) {
            jQuery.ajax({
                url: elem.src,
                async: false,
                dataType: "script"
            });
        } else {
            jQuery.globalEval((elem.text || elem.textContent || elem.innerHTML || "").replace(rcleanScript, "/*$0*/"));
        }

        if (elem.parentNode) {
            elem.parentNode.removeChild(elem);
        }
    }




    var ralpha = /alpha\([^)]*\)/i,
	ropacity = /opacity=([^)]*)/,
    // fixed for IE9, see #8346
	rupper = /([A-Z]|^ms)/g,
	rnumpx = /^-?\d+(?:px)?$/i,
	rnum = /^-?\d/,
	rrelNum = /^([\-+])=([\-+.\de]+)/,

	cssShow = { position: "absolute", visibility: "hidden", display: "block" },
	cssWidth = ["Left", "Right"],
	cssHeight = ["Top", "Bottom"],
	curCSS,

	getComputedStyle,
	currentStyle;

    jQuery.fn.css = function (name, value) {
        // Setting 'undefined' is a no-op
        if (arguments.length === 2 && value === undefined) {
            return this;
        }

        return jQuery.access(this, name, value, true, function (elem, name, value) {
            return value !== undefined ?
			jQuery.style(elem, name, value) :
			jQuery.css(elem, name);
        });
    };

    jQuery.extend({
        // Add in style property hooks for overriding the default
        // behavior of getting and setting a style property
        cssHooks: {
            opacity: {
                get: function (elem, computed) {
                    if (computed) {
                        // We should always get a number back from opacity
                        var ret = curCSS(elem, "opacity", "opacity");
                        return ret === "" ? "1" : ret;

                    } else {
                        return elem.style.opacity;
                    }
                }
            }
        },

        // Exclude the following css properties to add px
        cssNumber: {
            "fillOpacity": true,
            "fontWeight": true,
            "lineHeight": true,
            "opacity": true,
            "orphans": true,
            "widows": true,
            "zIndex": true,
            "zoom": true
        },

        // Add in properties whose names you wish to fix before
        // setting or getting the value
        cssProps: {
            // normalize float css property
            "float": jQuery.support.cssFloat ? "cssFloat" : "styleFloat"
        },

        // Get and set the style property on a DOM Node
        style: function (elem, name, value, extra) {
            // Don't set styles on text and comment nodes
            if (!elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style) {
                return;
            }

            // Make sure that we're working with the right name
            var ret, type, origName = jQuery.camelCase(name),
			style = elem.style, hooks = jQuery.cssHooks[origName];

            name = jQuery.cssProps[origName] || origName;

            // Check if we're setting a value
            if (value !== undefined) {
                type = typeof value;

                // convert relative number strings (+= or -=) to relative numbers. #7345
                if (type === "string" && (ret = rrelNum.exec(value))) {
                    value = (+(ret[1] + 1) * +ret[2]) + parseFloat(jQuery.css(elem, name));
                    // Fixes bug #9237
                    type = "number";
                }

                // Make sure that NaN and null values aren't set. See: #7116
                if (value == null || type === "number" && isNaN(value)) {
                    return;
                }

                // If a number was passed in, add 'px' to the (except for certain CSS properties)
                if (type === "number" && !jQuery.cssNumber[origName]) {
                    value += "px";
                }

                // If a hook was provided, use that value, otherwise just set the specified value
                if (!hooks || !("set" in hooks) || (value = hooks.set(elem, value)) !== undefined) {
                    // Wrapped to prevent IE from throwing errors when 'invalid' values are provided
                    // Fixes bug #5509
                    try {
                        style[name] = value;
                    } catch (e) { }
                }

            } else {
                // If a hook was provided get the non-computed value from there
                if (hooks && "get" in hooks && (ret = hooks.get(elem, false, extra)) !== undefined) {
                    return ret;
                }

                // Otherwise just get the value from the style object
                return style[name];
            }
        },

        css: function (elem, name, extra) {
            var ret, hooks;

            // Make sure that we're working with the right name
            name = jQuery.camelCase(name);
            hooks = jQuery.cssHooks[name];
            name = jQuery.cssProps[name] || name;

            // cssFloat needs a special treatment
            if (name === "cssFloat") {
                name = "float";
            }

            // If a hook was provided get the computed value from there
            if (hooks && "get" in hooks && (ret = hooks.get(elem, true, extra)) !== undefined) {
                return ret;

                // Otherwise, if a way to get the computed value exists, use that
            } else if (curCSS) {
                return curCSS(elem, name);
            }
        },

        // A method for quickly swapping in/out CSS properties to get correct calculations
        swap: function (elem, options, callback) {
            var old = {};

            // Remember the old values, and insert the new ones
            for (var name in options) {
                old[name] = elem.style[name];
                elem.style[name] = options[name];
            }

            callback.call(elem);

            // Revert the old values
            for (name in options) {
                elem.style[name] = old[name];
            }
        }
    });

    // DEPRECATED, Use jQuery.css() instead
    jQuery.curCSS = jQuery.css;

    jQuery.each(["height", "width"], function (i, name) {
        jQuery.cssHooks[name] = {
            get: function (elem, computed, extra) {
                var val;

                if (computed) {
                    if (elem.offsetWidth !== 0) {
                        return getWH(elem, name, extra);
                    } else {
                        jQuery.swap(elem, cssShow, function () {
                            val = getWH(elem, name, extra);
                        });
                    }

                    return val;
                }
            },

            set: function (elem, value) {
                if (rnumpx.test(value)) {
                    // ignore negative width and height values #1599
                    value = parseFloat(value);

                    if (value >= 0) {
                        return value + "px";
                    }

                } else {
                    return value;
                }
            }
        };
    });

    if (!jQuery.support.opacity) {
        jQuery.cssHooks.opacity = {
            get: function (elem, computed) {
                // IE uses filters for opacity
                return ropacity.test((computed && elem.currentStyle ? elem.currentStyle.filter : elem.style.filter) || "") ?
				(parseFloat(RegExp.$1) / 100) + "" :
				computed ? "1" : "";
            },

            set: function (elem, value) {
                var style = elem.style,
				currentStyle = elem.currentStyle,
				opacity = jQuery.isNumeric(value) ? "alpha(opacity=" + value * 100 + ")" : "",
				filter = currentStyle && currentStyle.filter || style.filter || "";

                // IE has trouble with opacity if it does not have layout
                // Force it by setting the zoom level
                style.zoom = 1;

                // if setting opacity to 1, and no other filters exist - attempt to remove filter attribute #6652
                if (value >= 1 && jQuery.trim(filter.replace(ralpha, "")) === "") {

                    // Setting style.filter to null, "" & " " still leave "filter:" in the cssText
                    // if "filter:" is present at all, clearType is disabled, we want to avoid this
                    // style.removeAttribute is IE Only, but so apparently is this code path...
                    style.removeAttribute("filter");

                    // if there there is no filter style applied in a css rule, we are done
                    if (currentStyle && !currentStyle.filter) {
                        return;
                    }
                }

                // otherwise, set new filter values
                style.filter = ralpha.test(filter) ?
				filter.replace(ralpha, opacity) :
				filter + " " + opacity;
            }
        };
    }

    jQuery(function () {
        // This hook cannot be added until DOM ready because the support test
        // for it is not run until after DOM ready
        if (!jQuery.support.reliableMarginRight) {
            jQuery.cssHooks.marginRight = {
                get: function (elem, computed) {
                    // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
                    // Work around by temporarily setting element display to inline-block
                    var ret;
                    jQuery.swap(elem, { "display": "inline-block" }, function () {
                        if (computed) {
                            ret = curCSS(elem, "margin-right", "marginRight");
                        } else {
                            ret = elem.style.marginRight;
                        }
                    });
                    return ret;
                }
            };
        }
    });

    if (document.defaultView && document.defaultView.getComputedStyle) {
        getComputedStyle = function (elem, name) {
            var ret, defaultView, computedStyle;

            name = name.replace(rupper, "-$1").toLowerCase();

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

    if (document.documentElement.currentStyle) {
        currentStyle = function (elem, name) {
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
            if (!rnumpx.test(ret) && rnum.test(ret)) {

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

    curCSS = getComputedStyle || currentStyle;

    function getWH(elem, name, extra) {

        // Start with offset property
        var val = name === "width" ? elem.offsetWidth : elem.offsetHeight,
		which = name === "width" ? cssWidth : cssHeight,
		i = 0,
		len = which.length;

        if (val > 0) {
            if (extra !== "border") {
                for (; i < len; i++) {
                    if (!extra) {
                        val -= parseFloat(jQuery.css(elem, "padding" + which[i])) || 0;
                    }
                    if (extra === "margin") {
                        val += parseFloat(jQuery.css(elem, extra + which[i])) || 0;
                    } else {
                        val -= parseFloat(jQuery.css(elem, "border" + which[i] + "Width")) || 0;
                    }
                }
            }

            return val + "px";
        }

        // Fall back to computed then uncomputed css if necessary
        val = curCSS(elem, name, name);
        if (val < 0 || val == null) {
            val = elem.style[name] || 0;
        }
        // Normalize "", auto, and prepare for extra
        val = parseFloat(val) || 0;

        // Add padding, border, margin
        if (extra) {
            for (; i < len; i++) {
                val += parseFloat(jQuery.css(elem, "padding" + which[i])) || 0;
                if (extra !== "padding") {
                    val += parseFloat(jQuery.css(elem, "border" + which[i] + "Width")) || 0;
                }
                if (extra === "margin") {
                    val += parseFloat(jQuery.css(elem, extra + which[i])) || 0;
                }
            }
        }

        return val + "px";
    }

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.hidden = function (elem) {
            var width = elem.offsetWidth,
			height = elem.offsetHeight;

            return (width === 0 && height === 0) || (!jQuery.support.reliableHiddenOffsets && ((elem.style && elem.style.display) || jQuery.css(elem, "display")) === "none");
        };

        jQuery.expr.filters.visible = function (elem) {
            return !jQuery.expr.filters.hidden(elem);
        };
    }




    var r20 = /%20/g,
	rbracket = /\[\]$/,
	rCRLF = /\r?\n/g,
	rhash = /#.*$/,
	rheaders = /^(.*?):[ \t]*([^\r\n]*)\r?$/mg, // IE leaves an \r character at EOL
	rinput = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i,
    // #7653, #8125, #8152: local protocol detection
	rlocalProtocol = /^(?:about|app|app\-storage|.+\-extension|file|res|widget):$/,
	rnoContent = /^(?:GET|HEAD)$/,
	rprotocol = /^\/\//,
	rquery = /\?/,
	rscript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi,
	rselectTextarea = /^(?:select|textarea)/i,
	rspacesAjax = /\s+/,
	rts = /([?&])_=[^&]*/,
	rurl = /^([\w\+\.\-]+:)(?:\/\/([^\/?#:]*)(?::(\d+))?)?/,

    // Keep a copy of the old load method
	_load = jQuery.fn.load,

    /* Prefilters
    * 1) They are useful to introduce custom dataTypes (see ajax/jsonp.js for an example)
    * 2) These are called:
    *    - BEFORE asking for a transport
    *    - AFTER param serialization (s.data is a string if s.processData is true)
    * 3) key is the dataType
    * 4) the catchall symbol "*" can be used
    * 5) execution will start with transport dataType and THEN continue down to "*" if needed
    */
	prefilters = {},

    /* Transports bindings
    * 1) key is the dataType
    * 2) the catchall symbol "*" can be used
    * 3) selection will start with transport dataType and THEN go to "*" if needed
    */
	transports = {},

    // Document location
	ajaxLocation,

    // Document location segments
	ajaxLocParts,

    // Avoid comment-prolog char sequence (#10098); must appease lint and evade compression
	allTypes = ["*/"] + ["*"];

    // #8138, IE may throw an exception when accessing
    // a field from window.location if document.domain has been set
    try {
        ajaxLocation = location.href;
    } catch (e) {
        // Use the href attribute of an A element
        // since IE will modify it given document.location
        ajaxLocation = document.createElement("a");
        ajaxLocation.href = "";
        ajaxLocation = ajaxLocation.href;
    }

    // Segment location into parts
    ajaxLocParts = rurl.exec(ajaxLocation.toLowerCase()) || [];

    // Base "constructor" for jQuery.ajaxPrefilter and jQuery.ajaxTransport
    function addToPrefiltersOrTransports(structure) {

        // dataTypeExpression is optional and defaults to "*"
        return function (dataTypeExpression, func) {

            if (typeof dataTypeExpression !== "string") {
                func = dataTypeExpression;
                dataTypeExpression = "*";
            }

            if (jQuery.isFunction(func)) {
                var dataTypes = dataTypeExpression.toLowerCase().split(rspacesAjax),
				i = 0,
				length = dataTypes.length,
				dataType,
				list,
				placeBefore;

                // For each dataType in the dataTypeExpression
                for (; i < length; i++) {
                    dataType = dataTypes[i];
                    // We control if we're asked to add before
                    // any existing element
                    placeBefore = /^\+/.test(dataType);
                    if (placeBefore) {
                        dataType = dataType.substr(1) || "*";
                    }
                    list = structure[dataType] = structure[dataType] || [];
                    // then we add to the structure accordingly
                    list[placeBefore ? "unshift" : "push"](func);
                }
            }
        };
    }

    // Base inspection function for prefilters and transports
    function inspectPrefiltersOrTransports(structure, options, originalOptions, jqXHR,
		dataType /* internal */, inspected /* internal */) {

        dataType = dataType || options.dataTypes[0];
        inspected = inspected || {};

        inspected[dataType] = true;

        var list = structure[dataType],
		i = 0,
		length = list ? list.length : 0,
		executeOnly = (structure === prefilters),
		selection;

        for (; i < length && (executeOnly || !selection); i++) {
            selection = list[i](options, originalOptions, jqXHR);
            // If we got redirected to another dataType
            // we try there if executing only and not done already
            if (typeof selection === "string") {
                if (!executeOnly || inspected[selection]) {
                    selection = undefined;
                } else {
                    options.dataTypes.unshift(selection);
                    selection = inspectPrefiltersOrTransports(
						structure, options, originalOptions, jqXHR, selection, inspected);
                }
            }
        }
        // If we're only executing or nothing was selected
        // we try the catchall dataType if not done already
        if ((executeOnly || !selection) && !inspected["*"]) {
            selection = inspectPrefiltersOrTransports(
				structure, options, originalOptions, jqXHR, "*", inspected);
        }
        // unnecessary when only executing (prefilters)
        // but it'll be ignored by the caller in that case
        return selection;
    }

    // A special extend for ajax options
    // that takes "flat" options (not to be deep extended)
    // Fixes #9887
    function ajaxExtend(target, src) {
        var key, deep,
		flatOptions = jQuery.ajaxSettings.flatOptions || {};
        for (key in src) {
            if (src[key] !== undefined) {
                (flatOptions[key] ? target : (deep || (deep = {})))[key] = src[key];
            }
        }
        if (deep) {
            jQuery.extend(true, target, deep);
        }
    }

    jQuery.fn.extend({
        load: function (url, params, callback) {
            if (typeof url !== "string" && _load) {
                return _load.apply(this, arguments);

                // Don't do a request if no elements are being requested
            } else if (!this.length) {
                return this;
            }

            var off = url.indexOf(" ");
            if (off >= 0) {
                var selector = url.slice(off, url.length);
                url = url.slice(0, off);
            }

            // Default to a GET request
            var type = "GET";

            // If the second parameter was provided
            if (params) {
                // If it's a function
                if (jQuery.isFunction(params)) {
                    // We assume that it's the callback
                    callback = params;
                    params = undefined;

                    // Otherwise, build a param string
                } else if (typeof params === "object") {
                    params = jQuery.param(params, jQuery.ajaxSettings.traditional);
                    type = "POST";
                }
            }

            var self = this;

            // Request the remote document
            jQuery.ajax({
                url: url,
                type: type,
                dataType: "html",
                data: params,
                // Complete callback (responseText is used internally)
                complete: function (jqXHR, status, responseText) {
                    // Store the response as specified by the jqXHR object
                    responseText = jqXHR.responseText;
                    // If successful, inject the HTML into all the matched elements
                    if (jqXHR.isResolved()) {
                        // #4825: Get the actual response in case
                        // a dataFilter is present in ajaxSettings
                        jqXHR.done(function (r) {
                            responseText = r;
                        });
                        // See if a selector was specified
                        self.html(selector ?
                        // Create a dummy div to hold the results
						jQuery("<div>")
                        // inject the contents of the document in, removing the scripts
                        // to avoid any 'Permission Denied' errors in IE
							.append(responseText.replace(rscript, ""))

                        // Locate the specified elements
							.find(selector) :

                        // If not, just inject the full result
						responseText);
                    }

                    if (callback) {
                        self.each(callback, [responseText, status, jqXHR]);
                    }
                }
            });

            return this;
        },

        serialize: function () {
            return jQuery.param(this.serializeArray());
        },

        serializeArray: function () {
            return this.map(function () {
                return this.elements ? jQuery.makeArray(this.elements) : this;
            })
		.filter(function () {
		    return this.name && !this.disabled &&
				(this.checked || rselectTextarea.test(this.nodeName) ||
					rinput.test(this.type));
		})
		.map(function (i, elem) {
		    var val = jQuery(this).val();

		    return val == null ?
				null :
				jQuery.isArray(val) ?
					jQuery.map(val, function (val, i) {
					    return { name: elem.name, value: val.replace(rCRLF, "\r\n") };
					}) :
					{ name: elem.name, value: val.replace(rCRLF, "\r\n") };
		}).get();
        }
    });

    // Attach a bunch of functions for handling common AJAX events
    jQuery.each("ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess ajaxSend".split(" "), function (i, o) {
        jQuery.fn[o] = function (f) {
            return this.on(o, f);
        };
    });

    jQuery.each(["get", "post"], function (i, method) {
        jQuery[method] = function (url, data, callback, type) {
            // shift arguments if data argument was omitted
            if (jQuery.isFunction(data)) {
                type = type || callback;
                callback = data;
                data = undefined;
            }

            return jQuery.ajax({
                type: method,
                url: url,
                data: data,
                success: callback,
                dataType: type
            });
        };
    });

    jQuery.extend({

        getScript: function (url, callback) {
            return jQuery.get(url, undefined, callback, "script");
        },

        getJSON: function (url, data, callback) {
            return jQuery.get(url, data, callback, "json");
        },

        // Creates a full fledged settings object into target
        // with both ajaxSettings and settings fields.
        // If target is omitted, writes into ajaxSettings.
        ajaxSetup: function (target, settings) {
            if (settings) {
                // Building a settings object
                ajaxExtend(target, jQuery.ajaxSettings);
            } else {
                // Extending ajaxSettings
                settings = target;
                target = jQuery.ajaxSettings;
            }
            ajaxExtend(target, settings);
            return target;
        },

        ajaxSettings: {
            url: ajaxLocation,
            isLocal: rlocalProtocol.test(ajaxLocParts[1]),
            global: true,
            type: "GET",
            contentType: "application/x-www-form-urlencoded",
            processData: true,
            async: true,
            /*
            timeout: 0,
            data: null,
            dataType: null,
            username: null,
            password: null,
            cache: null,
            traditional: false,
            headers: {},
            */

            accepts: {
                xml: "application/xml, text/xml",
                html: "text/html",
                text: "text/plain",
                json: "application/json, text/javascript",
                "*": allTypes
            },

            contents: {
                xml: /xml/,
                html: /html/,
                json: /json/
            },

            responseFields: {
                xml: "responseXML",
                text: "responseText"
            },

            // List of data converters
            // 1) key format is "source_type destination_type" (a single space in-between)
            // 2) the catchall symbol "*" can be used for source_type
            converters: {

                // Convert anything to text
                "* text": window.String,

                // Text to html (true = no transformation)
                "text html": true,

                // Evaluate text as a json expression
                "text json": jQuery.parseJSON,

                // Parse text as xml
                "text xml": jQuery.parseXML
            },

            // For options that shouldn't be deep extended:
            // you can add your own custom options here if
            // and when you create one that shouldn't be
            // deep extended (see ajaxExtend)
            flatOptions: {
                context: true,
                url: true
            }
        },

        ajaxPrefilter: addToPrefiltersOrTransports(prefilters),
        ajaxTransport: addToPrefiltersOrTransports(transports),

        // Main method
        ajax: function (url, options) {

            // If url is an object, simulate pre-1.5 signature
            if (typeof url === "object") {
                options = url;
                url = undefined;
            }

            // Force options to be an object
            options = options || {};

            var // Create the final options object
			s = jQuery.ajaxSetup({}, options),
            // Callbacks context
			callbackContext = s.context || s,
            // Context for global events
            // It's the callbackContext if one was provided in the options
            // and if it's a DOM node or a jQuery collection
			globalEventContext = callbackContext !== s &&
				(callbackContext.nodeType || callbackContext instanceof jQuery) ?
						jQuery(callbackContext) : jQuery.event,
            // Deferreds
			deferred = jQuery.Deferred(),
			completeDeferred = jQuery.Callbacks("once memory"),
            // Status-dependent callbacks
			statusCode = s.statusCode || {},
            // ifModified key
			ifModifiedKey,
            // Headers (they are sent all at once)
			requestHeaders = {},
			requestHeadersNames = {},
            // Response headers
			responseHeadersString,
			responseHeaders,
            // transport
			transport,
            // timeout handle
			timeoutTimer,
            // Cross-domain detection vars
			parts,
            // The jqXHR state
			state = 0,
            // To know if global events are to be dispatched
			fireGlobals,
            // Loop variable
			i,
            // Fake xhr
			jqXHR = {

			    readyState: 0,

			    // Caches the header
			    setRequestHeader: function (name, value) {
			        if (!state) {
			            var lname = name.toLowerCase();
			            name = requestHeadersNames[lname] = requestHeadersNames[lname] || name;
			            requestHeaders[name] = value;
			        }
			        return this;
			    },

			    // Raw string
			    getAllResponseHeaders: function () {
			        return state === 2 ? responseHeadersString : null;
			    },

			    // Builds headers hashtable if needed
			    getResponseHeader: function (key) {
			        var match;
			        if (state === 2) {
			            if (!responseHeaders) {
			                responseHeaders = {};
			                while ((match = rheaders.exec(responseHeadersString))) {
			                    responseHeaders[match[1].toLowerCase()] = match[2];
			                }
			            }
			            match = responseHeaders[key.toLowerCase()];
			        }
			        return match === undefined ? null : match;
			    },

			    // Overrides response content-type header
			    overrideMimeType: function (type) {
			        if (!state) {
			            s.mimeType = type;
			        }
			        return this;
			    },

			    // Cancel the request
			    abort: function (statusText) {
			        statusText = statusText || "abort";
			        if (transport) {
			            transport.abort(statusText);
			        }
			        done(0, statusText);
			        return this;
			    }
			};

            // Callback for when everything is done
            // It is defined here because jslint complains if it is declared
            // at the end of the function (which would be more logical and readable)
            function done(status, nativeStatusText, responses, headers) {

                // Called once
                if (state === 2) {
                    return;
                }

                // State is "done" now
                state = 2;

                // Clear timeout if it exists
                if (timeoutTimer) {
                    clearTimeout(timeoutTimer);
                }

                // Dereference transport for early garbage collection
                // (no matter how long the jqXHR object will be used)
                transport = undefined;

                // Cache response headers
                responseHeadersString = headers || "";

                // Set readyState
                jqXHR.readyState = status > 0 ? 4 : 0;

                var isSuccess,
				success,
				error,
				statusText = nativeStatusText,
				response = responses ? ajaxHandleResponses(s, jqXHR, responses) : undefined,
				lastModified,
				etag;

                // If successful, handle type chaining
                if (status >= 200 && status < 300 || status === 304) {

                    // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
                    if (s.ifModified) {

                        if ((lastModified = jqXHR.getResponseHeader("Last-Modified"))) {
                            jQuery.lastModified[ifModifiedKey] = lastModified;
                        }
                        if ((etag = jqXHR.getResponseHeader("Etag"))) {
                            jQuery.etag[ifModifiedKey] = etag;
                        }
                    }

                    // If not modified
                    if (status === 304) {

                        statusText = "notmodified";
                        isSuccess = true;

                        // If we have data
                    } else {

                        try {
                            success = ajaxConvert(s, response);
                            statusText = "success";
                            isSuccess = true;
                        } catch (e) {
                            // We have a parsererror
                            statusText = "parsererror";
                            error = e;
                        }
                    }
                } else {
                    // We extract error from statusText
                    // then normalize statusText and status for non-aborts
                    error = statusText;
                    if (!statusText || status) {
                        statusText = "error";
                        if (status < 0) {
                            status = 0;
                        }
                    }
                }

                // Set data for the fake xhr object
                jqXHR.status = status;
                jqXHR.statusText = "" + (nativeStatusText || statusText);

                // Success/Error
                if (isSuccess) {
                    deferred.resolveWith(callbackContext, [success, statusText, jqXHR]);
                } else {
                    deferred.rejectWith(callbackContext, [jqXHR, statusText, error]);
                }

                // Status-dependent callbacks
                jqXHR.statusCode(statusCode);
                statusCode = undefined;

                if (fireGlobals) {
                    globalEventContext.trigger("ajax" + (isSuccess ? "Success" : "Error"),
						[jqXHR, s, isSuccess ? success : error]);
                }

                // Complete
                completeDeferred.fireWith(callbackContext, [jqXHR, statusText]);

                if (fireGlobals) {
                    globalEventContext.trigger("ajaxComplete", [jqXHR, s]);
                    // Handle the global AJAX counter
                    if (!(--jQuery.active)) {
                        jQuery.event.trigger("ajaxStop");
                    }
                }
            }

            // Attach deferreds
            deferred.promise(jqXHR);
            jqXHR.success = jqXHR.done;
            jqXHR.error = jqXHR.fail;
            jqXHR.complete = completeDeferred.add;

            // Status-dependent callbacks
            jqXHR.statusCode = function (map) {
                if (map) {
                    var tmp;
                    if (state < 2) {
                        for (tmp in map) {
                            statusCode[tmp] = [statusCode[tmp], map[tmp]];
                        }
                    } else {
                        tmp = map[jqXHR.status];
                        jqXHR.then(tmp, tmp);
                    }
                }
                return this;
            };

            // Remove hash character (#7531: and string promotion)
            // Add protocol if not provided (#5866: IE7 issue with protocol-less urls)
            // We also use the url parameter if available
            s.url = ((url || s.url) + "").replace(rhash, "").replace(rprotocol, ajaxLocParts[1] + "//");

            // Extract dataTypes list
            s.dataTypes = jQuery.trim(s.dataType || "*").toLowerCase().split(rspacesAjax);

            // Determine if a cross-domain request is in order
            if (s.crossDomain == null) {
                parts = rurl.exec(s.url.toLowerCase());
                s.crossDomain = !!(parts &&
				(parts[1] != ajaxLocParts[1] || parts[2] != ajaxLocParts[2] ||
					(parts[3] || (parts[1] === "http:" ? 80 : 443)) !=
						(ajaxLocParts[3] || (ajaxLocParts[1] === "http:" ? 80 : 443)))
			);
            }

            // Convert data if not already a string
            if (s.data && s.processData && typeof s.data !== "string") {
                s.data = jQuery.param(s.data, s.traditional);
            }

            // Apply prefilters
            inspectPrefiltersOrTransports(prefilters, s, options, jqXHR);

            // If request was aborted inside a prefiler, stop there
            if (state === 2) {
                return false;
            }

            // We can fire global events as of now if asked to
            fireGlobals = s.global;

            // Uppercase the type
            s.type = s.type.toUpperCase();

            // Determine if request has content
            s.hasContent = !rnoContent.test(s.type);

            // Watch for a new set of requests
            if (fireGlobals && jQuery.active++ === 0) {
                jQuery.event.trigger("ajaxStart");
            }

            // More options handling for requests with no content
            if (!s.hasContent) {

                // If data is available, append data to url
                if (s.data) {
                    s.url += (rquery.test(s.url) ? "&" : "?") + s.data;
                    // #9682: remove data so that it's not used in an eventual retry
                    delete s.data;
                }

                // Get ifModifiedKey before adding the anti-cache parameter
                ifModifiedKey = s.url;

                // Add anti-cache in url if needed
                if (s.cache === false) {

                    var ts = jQuery.now(),
                    // try replacing _= if it is there
					ret = s.url.replace(rts, "$1_=" + ts);

                    // if nothing was replaced, add timestamp to the end
                    s.url = ret + ((ret === s.url) ? (rquery.test(s.url) ? "&" : "?") + "_=" + ts : "");
                }
            }

            // Set the correct header, if data is being sent
            if (s.data && s.hasContent && s.contentType !== false || options.contentType) {
                jqXHR.setRequestHeader("Content-Type", s.contentType);
            }

            // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
            if (s.ifModified) {
                ifModifiedKey = ifModifiedKey || s.url;
                if (jQuery.lastModified[ifModifiedKey]) {
                    jqXHR.setRequestHeader("If-Modified-Since", jQuery.lastModified[ifModifiedKey]);
                }
                if (jQuery.etag[ifModifiedKey]) {
                    jqXHR.setRequestHeader("If-None-Match", jQuery.etag[ifModifiedKey]);
                }
            }

            // Set the Accepts header for the server, depending on the dataType
            jqXHR.setRequestHeader(
			"Accept",
			s.dataTypes[0] && s.accepts[s.dataTypes[0]] ?
				s.accepts[s.dataTypes[0]] + (s.dataTypes[0] !== "*" ? ", " + allTypes + "; q=0.01" : "") :
				s.accepts["*"]
		);

            // Check for headers option
            for (i in s.headers) {
                jqXHR.setRequestHeader(i, s.headers[i]);
            }

            // Allow custom headers/mimetypes and early abort
            if (s.beforeSend && (s.beforeSend.call(callbackContext, jqXHR, s) === false || state === 2)) {
                // Abort if not done already
                jqXHR.abort();
                return false;

            }

            // Install callbacks on deferreds
            for (i in { success: 1, error: 1, complete: 1 }) {
                jqXHR[i](s[i]);
            }

            // Get transport
            transport = inspectPrefiltersOrTransports(transports, s, options, jqXHR);

            // If no transport, we auto-abort
            if (!transport) {
                done(-1, "No Transport");
            } else {
                jqXHR.readyState = 1;
                // Send global event
                if (fireGlobals) {
                    globalEventContext.trigger("ajaxSend", [jqXHR, s]);
                }
                jv.CreateEventSource(callbackContext);
                // Timeout
                if (s.async && s.timeout > 0) {
                    timeoutTimer = setTimeout(function () {
                        jqXHR.abort("timeout");
                    }, s.timeout);
                }

                try {
                    state = 1;
                    transport.send(requestHeaders, done);
                } catch (e) {
                    // Propagate exception as error if not done
                    if (state < 2) {
                        done(-1, e);
                        // Simply rethrow otherwise
                    } else {
                        throw e;
                    }
                }
            }

            return jqXHR;
        },

        // Serialize an array of form elements or a set of
        // key/values into a query string
        param: function (a, traditional) {
            var s = [],
			add = function (key, value) {
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
            return s.join("&").replace(r20, "+");
        }
    });

    function buildParams(prefix, obj, traditional, add) {
        if (jQuery.isArray(obj)) {
            // Serialize array item.
            jQuery.each(obj, function (i, v) {
                if (traditional || rbracket.test(prefix)) {
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
                    buildParams(prefix + "[" + (typeof v === "object" || jQuery.isArray(v) ? i : "") + "]", v, traditional, add);
                }
            });

        } else if (!traditional && obj != null && typeof obj === "object") {
            // Serialize object item.
            for (var name in obj) {
                buildParams(prefix + "[" + name + "]", obj[name], traditional, add);
            }

        } else {
            // Serialize scalar item.
            add(prefix, obj);
        }
    }

    // This is still on the jQuery object... for now
    // Want to move this to jQuery.ajax some day
    jQuery.extend({

        // Counter for holding the number of active queries
        active: 0,

        // Last-Modified header cache for next request
        lastModified: {},
        etag: {}

    });

    /* Handles responses to an ajax request:
    * - sets all responseXXX fields accordingly
    * - finds the right dataType (mediates between content-type and expected dataType)
    * - returns the corresponding response
    */
    function ajaxHandleResponses(s, jqXHR, responses) {

        var contents = s.contents,
		dataTypes = s.dataTypes,
		responseFields = s.responseFields,
		ct,
		type,
		finalDataType,
		firstDataType;

        // Fill responseXXX fields
        for (type in responseFields) {
            if (type in responses) {
                jqXHR[responseFields[type]] = responses[type];
            }
        }

        // Remove auto dataType and get content-type in the process
        while (dataTypes[0] === "*") {
            dataTypes.shift();
            if (ct === undefined) {
                ct = s.mimeType || jqXHR.getResponseHeader("content-type");
            }
        }

        // Check if we're dealing with a known content-type
        if (ct) {
            for (type in contents) {
                if (contents[type] && contents[type].test(ct)) {
                    dataTypes.unshift(type);
                    break;
                }
            }
        }

        // Check to see if we have a response for the expected dataType
        if (dataTypes[0] in responses) {
            finalDataType = dataTypes[0];
        } else {
            // Try convertible dataTypes
            for (type in responses) {
                if (!dataTypes[0] || s.converters[type + " " + dataTypes[0]]) {
                    finalDataType = type;
                    break;
                }
                if (!firstDataType) {
                    firstDataType = type;
                }
            }
            // Or just use first one
            finalDataType = finalDataType || firstDataType;
        }

        // If we found a dataType
        // We add the dataType to the list if needed
        // and return the corresponding response
        if (finalDataType) {
            if (finalDataType !== dataTypes[0]) {
                dataTypes.unshift(finalDataType);
            }
            return responses[finalDataType];
        }
    }

    // Chain conversions given the request and the original response
    function ajaxConvert(s, response) {

        // Apply the dataFilter if provided
        if (s.dataFilter) {
            response = s.dataFilter(response, s.dataType);
        }

        var dataTypes = s.dataTypes,
		converters = {},
		i,
		key,
		length = dataTypes.length,
		tmp,
        // Current and previous dataTypes
		current = dataTypes[0],
		prev,
        // Conversion expression
		conversion,
        // Conversion function
		conv,
        // Conversion functions (transitive conversion)
		conv1,
		conv2;

        // For each dataType in the chain
        for (i = 1; i < length; i++) {

            // Create converters map
            // with lowercased keys
            if (i === 1) {
                for (key in s.converters) {
                    if (typeof key === "string") {
                        converters[key.toLowerCase()] = s.converters[key];
                    }
                }
            }

            // Get the dataTypes
            prev = current;
            current = dataTypes[i];

            // If current is auto dataType, update it to prev
            if (current === "*") {
                current = prev;
                // If no auto and dataTypes are actually different
            } else if (prev !== "*" && prev !== current) {

                // Get the converter
                conversion = prev + " " + current;
                conv = converters[conversion] || converters["* " + current];

                // If there is no direct converter, search transitively
                if (!conv) {
                    conv2 = undefined;
                    for (conv1 in converters) {
                        tmp = conv1.split(" ");
                        if (tmp[0] === prev || tmp[0] === "*") {
                            conv2 = converters[tmp[1] + " " + current];
                            if (conv2) {
                                conv1 = converters[conv1];
                                if (conv1 === true) {
                                    conv = conv2;
                                } else if (conv2 === true) {
                                    conv = conv1;
                                }
                                break;
                            }
                        }
                    }
                }
                // If we found no converter, dispatch an error
                if (!(conv || conv2)) {
                    jQuery.error("No conversion from " + conversion.replace(" ", " to "));
                }
                // If found converter is not an equivalence
                if (conv !== true) {
                    // Convert with 1 or 2 converters accordingly
                    response = conv ? conv(response) : conv2(conv1(response));
                }
            }
        }
        return response;
    }




    var jsc = jQuery.now(),
	jsre = /(\=)\?(&|$)|\?\?/i;

    // Default jsonp settings
    jQuery.ajaxSetup({
        jsonp: "callback",
        jsonpCallback: function () {
            return jQuery.expando + "_" + (jsc++);
        }
    });

    // Detect, normalize options and install callbacks for jsonp requests
    jQuery.ajaxPrefilter("json jsonp", function (s, originalSettings, jqXHR) {

        var inspectData = s.contentType === "application/x-www-form-urlencoded" &&
		(typeof s.data === "string");

        if (s.dataTypes[0] === "jsonp" ||
		s.jsonp !== false && (jsre.test(s.url) ||
				inspectData && jsre.test(s.data))) {

            var responseContainer,
			jsonpCallback = s.jsonpCallback =
				jQuery.isFunction(s.jsonpCallback) ? s.jsonpCallback() : s.jsonpCallback,
			previous = window[jsonpCallback],
			url = s.url,
			data = s.data,
			replace = "$1" + jsonpCallback + "$2";

            if (s.jsonp !== false) {
                url = url.replace(jsre, replace);
                if (s.url === url) {
                    if (inspectData) {
                        data = data.replace(jsre, replace);
                    }
                    if (s.data === data) {
                        // Add callback manually
                        url += (/\?/.test(url) ? "&" : "?") + s.jsonp + "=" + jsonpCallback;
                    }
                }
            }

            s.url = url;
            s.data = data;

            // Install callback
            window[jsonpCallback] = function (response) {
                responseContainer = [response];
            };

            // Clean-up function
            jqXHR.always(function () {
                // Set callback back to previous value
                window[jsonpCallback] = previous;
                // Call if it was a function and we have a response
                if (responseContainer && jQuery.isFunction(previous)) {
                    window[jsonpCallback](responseContainer[0]);
                }
            });

            // Use data converter to retrieve json after script execution
            s.converters["script json"] = function () {
                if (!responseContainer) {
                    jQuery.error(jsonpCallback + " was not called");
                }
                return responseContainer[0];
            };

            // force json dataType
            s.dataTypes[0] = "json";

            // Delegate to script
            return "script";
        }
    });




    // Install script dataType
    jQuery.ajaxSetup({
        accepts: {
            script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"
        },
        contents: {
            script: /javascript|ecmascript/
        },
        converters: {
            "text script": function (text) {
                jQuery.globalEval(text);
                return text;
            }
        }
    });

    // Handle cache's special case and global
    jQuery.ajaxPrefilter("script", function (s) {
        if (s.cache === undefined) {
            s.cache = false;
        }
        if (s.crossDomain) {
            s.type = "GET";
            s.global = false;
        }
    });

    // Bind script tag hack transport
    jQuery.ajaxTransport("script", function (s) {

        // This transport only deals with cross domain requests
        if (s.crossDomain) {

            var script,
			head = document.head || document.getElementsByTagName("head")[0] || document.documentElement;

            return {

                send: function (_, callback) {

                    script = document.createElement("script");

                    script.async = "async";

                    if (s.scriptCharset) {
                        script.charset = s.scriptCharset;
                    }

                    script.src = s.url;

                    // Attach handlers for all browsers
                    script.onload = script.onreadystatechange = function (_, isAbort) {

                        if (isAbort || !script.readyState || /loaded|complete/.test(script.readyState)) {

                            // Handle memory leak in IE
                            script.onload = script.onreadystatechange = null;

                            // Remove the script
                            if (head && script.parentNode) {
                                head.removeChild(script);
                            }

                            // Dereference the script
                            script = undefined;

                            // Callback if not abort
                            if (!isAbort) {
                                callback(200, "success");
                            }
                        }
                    };
                    // Use insertBefore instead of appendChild  to circumvent an IE6 bug.
                    // This arises when a base node is used (#2709 and #4378).
                    head.insertBefore(script, head.firstChild);
                },

                abort: function () {
                    if (script) {
                        script.onload(0, 1);
                    }
                }
            };
        }
    });




    var // #5280: Internet Explorer will keep connections alive if we don't abort on unload
	xhrOnUnloadAbort = window.ActiveXObject ? function () {
	    // Abort all pending requests
	    for (var key in xhrCallbacks) {
	        xhrCallbacks[key](0, 1);
	    }
	} : false,
	xhrId = 0,
	xhrCallbacks;

    // Functions to create xhrs
    function createStandardXHR() {
        try {
            return new window.XMLHttpRequest();
        } catch (e) { }
    }

    function createActiveXHR() {
        try {
            return new window.ActiveXObject("Microsoft.XMLHTTP");
        } catch (e) { }
    }

    // Create the request object
    // (This is still attached to ajaxSettings for backward compatibility)
    jQuery.ajaxSettings.xhr = window.ActiveXObject ?
    /* Microsoft failed to properly
    * implement the XMLHttpRequest in IE7 (can't request local files),
    * so we use the ActiveXObject when it is available
    * Additionally XMLHttpRequest can be disabled in IE7/IE8 so
    * we need a fallback.
    */
	function () {
	    return !this.isLocal && createStandardXHR() || createActiveXHR();
	} :
    // For all other browsers, use the standard XMLHttpRequest object
	createStandardXHR;

    // Determine support properties
    (function (xhr) {
        jQuery.extend(jQuery.support, {
            ajax: !!xhr,
            cors: !!xhr && ("withCredentials" in xhr)
        });
    })(jQuery.ajaxSettings.xhr());

    // Create transport if the browser can provide an xhr
    if (jQuery.support.ajax) {

        jQuery.ajaxTransport(function (s) {
            // Cross domain only allowed if supported through XMLHttpRequest
            if (!s.crossDomain || jQuery.support.cors) {

                var callback;

                return {
                    send: function (headers, complete) {

                        // Get a new xhr
                        var xhr = s.xhr(),
						handle,
						i;

                        // Open the socket
                        // Passing null username, generates a login popup on Opera (#2865)
                        if (s.username) {
                            xhr.open(s.type, s.url, s.async, s.username, s.password);
                        } else {
                            xhr.open(s.type, s.url, s.async);
                        }

                        // Apply custom fields if provided
                        if (s.xhrFields) {
                            for (i in s.xhrFields) {
                                xhr[i] = s.xhrFields[i];
                            }
                        }

                        // Override mime type if needed
                        if (s.mimeType && xhr.overrideMimeType) {
                            xhr.overrideMimeType(s.mimeType);
                        }

                        // X-Requested-With header
                        // For cross-domain requests, seeing as conditions for a preflight are
                        // akin to a jigsaw puzzle, we simply never set it to be sure.
                        // (it can always be set on a per-request basis or even using ajaxSetup)
                        // For same-domain requests, won't change header if already provided.
                        if (!s.crossDomain && !headers["X-Requested-With"]) {
                            headers["X-Requested-With"] = "XMLHttpRequest";
                        }

                        // Need an extra try/catch for cross domain requests in Firefox 3
                        try {
                            for (i in headers) {
                                xhr.setRequestHeader(i, headers[i]);
                            }
                        } catch (_) { }

                        // Do send the request
                        // This may raise an exception which is actually
                        // handled in jQuery.ajax (so no try/catch here)
                        xhr.send((s.hasContent && s.data) || null);

                        // Listener
                        callback = function (_, isAbort) {

                            var status,
							statusText,
							responseHeaders,
							responses,
							xml;

                            // Firefox throws exceptions when accessing properties
                            // of an xhr when a network error occured
                            // http://helpful.knobs-dials.com/index.php/Component_returned_failure_code:_0x80040111_(NS_ERROR_NOT_AVAILABLE)
                            try {

                                // Was never called and is aborted or complete
                                if (callback && (isAbort || xhr.readyState === 4)) {

                                    // Only called once
                                    callback = undefined;

                                    // Do not keep as active anymore
                                    if (handle) {
                                        xhr.onreadystatechange = jQuery.noop;
                                        if (xhrOnUnloadAbort) {
                                            delete xhrCallbacks[handle];
                                        }
                                    }

                                    // If it's an abort
                                    if (isAbort) {
                                        // Abort it manually if needed
                                        if (xhr.readyState !== 4) {
                                            xhr.abort();
                                        }
                                    } else {
                                        status = xhr.status;
                                        responseHeaders = xhr.getAllResponseHeaders();
                                        responses = {};
                                        xml = xhr.responseXML;

                                        // Construct response list
                                        if (xml && xml.documentElement /* #4958 */) {
                                            responses.xml = xml;
                                        }
                                        responses.text = xhr.responseText;

                                        // Firefox throws an exception when accessing
                                        // statusText for faulty cross-domain requests
                                        try {
                                            statusText = xhr.statusText;
                                        } catch (e) {
                                            // We normalize with Webkit giving an empty statusText
                                            statusText = "";
                                        }

                                        // Filter status for non standard behaviors

                                        // If the request is local and we have data: assume a success
                                        // (success with no data won't get notified, that's the best we
                                        // can do given current implementations)
                                        if (!status && s.isLocal && !s.crossDomain) {
                                            status = responses.text ? 200 : 404;
                                            // IE - #1450: sometimes returns 1223 when it should be 204
                                        } else if (status === 1223) {
                                            status = 204;
                                        }
                                    }
                                }
                            } catch (firefoxAccessException) {
                                if (!isAbort) {
                                    complete(-1, firefoxAccessException);
                                }
                            }

                            // Call complete if needed
                            if (responses) {
                                complete(status, statusText, responses, responseHeaders);
                            }
                        };

                        // if we're in sync mode or it's in cache
                        // and has been retrieved directly (IE6 & IE7)
                        // we need to manually fire the callback
                        if (!s.async || xhr.readyState === 4) {
                            callback();
                        } else {
                            handle = ++xhrId;
                            if (xhrOnUnloadAbort) {
                                // Create the active xhrs callbacks list if needed
                                // and attach the unload handler
                                if (!xhrCallbacks) {
                                    xhrCallbacks = {};
                                    jQuery(window).unload(xhrOnUnloadAbort);
                                }
                                // Add to list of active xhrs callbacks
                                xhrCallbacks[handle] = callback;
                            }
                            xhr.onreadystatechange = callback;
                        }
                    },

                    abort: function () {
                        if (callback) {
                            callback(0, 1);
                        }
                    }
                };
            }
        });
    }




    var elemdisplay = {},
	iframe, iframeDoc,
	rfxtypes = /^(?:toggle|show|hide)$/,
	rfxnum = /^([+\-]=)?([\d+.\-]+)([a-z%]*)$/i,
	timerId,
	fxAttrs = [
    // height animations
		["height", "marginTop", "marginBottom", "paddingTop", "paddingBottom"],
    // width animations
		["width", "marginLeft", "marginRight", "paddingLeft", "paddingRight"],
    // opacity animations
		["opacity"]
	],
	fxNow;

    jQuery.fn.extend({
        show: function (speed, easing, callback) {
            var elem, display;

            if (speed || speed === 0) {
                return this.animate(genFx("show", 3), speed, easing, callback);

            } else {
                for (var i = 0, j = this.length; i < j; i++) {
                    elem = this[i];

                    if (elem.style) {
                        display = elem.style.display;

                        // Reset the inline display of this element to learn if it is
                        // being hidden by cascaded rules or not
                        if (!jQuery._data(elem, "olddisplay") && display === "none") {
                            display = elem.style.display = "";
                        }

                        // Set elements which have been overridden with display: none
                        // in a stylesheet to whatever the default browser style is
                        // for such an element
                        if (display === "" && jQuery.css(elem, "display") === "none") {
                            jQuery._data(elem, "olddisplay", defaultDisplay(elem.nodeName));
                        }
                    }
                }

                // Set the display of most of the elements in a second loop
                // to avoid the constant reflow
                for (i = 0; i < j; i++) {
                    elem = this[i];

                    if (elem.style) {
                        display = elem.style.display;

                        if (display === "" || display === "none") {
                            elem.style.display = jQuery._data(elem, "olddisplay") || "";
                        }
                    }
                }

                return this;
            }
        },

        hide: function (speed, easing, callback) {
            if (speed || speed === 0) {
                return this.animate(genFx("hide", 3), speed, easing, callback);

            } else {
                var elem, display,
				i = 0,
				j = this.length;

                for (; i < j; i++) {
                    elem = this[i];
                    if (elem.style) {
                        display = jQuery.css(elem, "display");

                        if (display !== "none" && !jQuery._data(elem, "olddisplay")) {
                            jQuery._data(elem, "olddisplay", display);
                        }
                    }
                }

                // Set the display of the elements in a second loop
                // to avoid the constant reflow
                for (i = 0; i < j; i++) {
                    if (this[i].style) {
                        this[i].style.display = "none";
                    }
                }

                return this;
            }
        },

        // Save the old toggle function
        _toggle: jQuery.fn.toggle,

        toggle: function (fn, fn2, callback) {
            var bool = typeof fn === "boolean";

            if (jQuery.isFunction(fn) && jQuery.isFunction(fn2)) {
                this._toggle.apply(this, arguments);

            } else if (fn == null || bool) {
                this.each(function () {
                    var state = bool ? fn : jQuery(this).is(":hidden");
                    jQuery(this)[state ? "show" : "hide"]();
                });

            } else {
                this.animate(genFx("toggle", 3), fn, fn2, callback);
            }

            return this;
        },

        fadeTo: function (speed, to, easing, callback) {
            return this.filter(":hidden").css("opacity", 0).show().end()
					.animate({ opacity: to }, speed, easing, callback);
        },

        animate: function (prop, speed, easing, callback) {
            var optall = jQuery.speed(speed, easing, callback);

            if (jQuery.isEmptyObject(prop)) {
                return this.each(optall.complete, [false]);
            }

            // Do not change referenced properties as per-property easing will be lost
            prop = jQuery.extend({}, prop);

            function doAnimation() {
                // XXX 'this' does not always have a nodeName when running the
                // test suite

                if (optall.queue === false) {
                    jQuery._mark(this);
                }

                var opt = jQuery.extend({}, optall),
				isElement = this.nodeType === 1,
				hidden = isElement && jQuery(this).is(":hidden"),
				name, val, p, e,
				parts, start, end, unit,
				method;

                // will store per property easing and be used to determine when an animation is complete
                opt.animatedProperties = {};

                for (p in prop) {

                    // property name normalization
                    name = jQuery.camelCase(p);
                    if (p !== name) {
                        prop[name] = prop[p];
                        delete prop[p];
                    }

                    val = prop[name];

                    // easing resolution: per property > opt.specialEasing > opt.easing > 'swing' (default)
                    if (jQuery.isArray(val)) {
                        opt.animatedProperties[name] = val[1];
                        val = prop[name] = val[0];
                    } else {
                        opt.animatedProperties[name] = opt.specialEasing && opt.specialEasing[name] || opt.easing || 'swing';
                    }

                    if (val === "hide" && hidden || val === "show" && !hidden) {
                        return opt.complete.call(this);
                    }

                    if (isElement && (name === "height" || name === "width")) {
                        // Make sure that nothing sneaks out
                        // Record all 3 overflow attributes because IE does not
                        // change the overflow attribute when overflowX and
                        // overflowY are set to the same value
                        opt.overflow = [this.style.overflow, this.style.overflowX, this.style.overflowY];

                        // Set display property to inline-block for height/width
                        // animations on inline elements that are having width/height animated
                        if (jQuery.css(this, "display") === "inline" &&
							jQuery.css(this, "float") === "none") {

                            // inline-level elements accept inline-block;
                            // block-level elements need to be inline with layout
                            if (!jQuery.support.inlineBlockNeedsLayout || defaultDisplay(this.nodeName) === "inline") {
                                this.style.display = "inline-block";

                            } else {
                                this.style.zoom = 1;
                            }
                        }
                    }
                }

                if (opt.overflow != null) {
                    this.style.overflow = "hidden";
                }

                for (p in prop) {
                    e = new jQuery.fx(this, opt, p);
                    val = prop[p];

                    if (rfxtypes.test(val)) {

                        // Tracks whether to show or hide based on private
                        // data attached to the element
                        method = jQuery._data(this, "toggle" + p) || (val === "toggle" ? hidden ? "show" : "hide" : 0);
                        if (method) {
                            jQuery._data(this, "toggle" + p, method === "show" ? "hide" : "show");
                            e[method]();
                        } else {
                            e[val]();
                        }

                    } else {
                        parts = rfxnum.exec(val);
                        start = e.cur();

                        if (parts) {
                            end = parseFloat(parts[2]);
                            unit = parts[3] || (jQuery.cssNumber[p] ? "" : "px");

                            // We need to compute starting value
                            if (unit !== "px") {
                                jQuery.style(this, p, (end || 1) + unit);
                                start = ((end || 1) / e.cur()) * start;
                                jQuery.style(this, p, start + unit);
                            }

                            // If a +=/-= token was provided, we're doing a relative animation
                            if (parts[1]) {
                                end = ((parts[1] === "-=" ? -1 : 1) * end) + start;
                            }

                            e.custom(start, end, unit);

                        } else {
                            e.custom(start, val, "");
                        }
                    }
                }

                // For JS strict compliance
                return true;
            }

            return optall.queue === false ?
			this.each(doAnimation) :
			this.queue(optall.queue, doAnimation);
        },

        stop: function (type, clearQueue, gotoEnd) {
            if (typeof type !== "string") {
                gotoEnd = clearQueue;
                clearQueue = type;
                type = undefined;
            }
            if (clearQueue && type !== false) {
                this.queue(type || "fx", []);
            }

            return this.each(function () {
                var index,
				hadTimers = false,
				timers = jQuery.timers,
				data = jQuery._data(this);

                // clear marker counters if we know they won't be
                if (!gotoEnd) {
                    jQuery._unmark(true, this);
                }

                function stopQueue(elem, data, index) {
                    var hooks = data[index];
                    jQuery.removeData(elem, index, true);
                    hooks.stop(gotoEnd);
                }

                if (type == null) {
                    for (index in data) {
                        if (data[index] && data[index].stop && index.indexOf(".run") === index.length - 4) {
                            stopQueue(this, data, index);
                        }
                    }
                } else if (data[index = type + ".run"] && data[index].stop) {
                    stopQueue(this, data, index);
                }

                for (index = timers.length; index--; ) {
                    if (timers[index].elem === this && (type == null || timers[index].queue === type)) {
                        if (gotoEnd) {

                            // force the next step to be the last
                            timers[index](true);
                        } else {
                            timers[index].saveState();
                        }
                        hadTimers = true;
                        timers.splice(index, 1);
                    }
                }

                // start the next in the queue if the last step wasn't forced
                // timers currently will call their complete callbacks, which will dequeue
                // but only if they were gotoEnd
                if (!(gotoEnd && hadTimers)) {
                    jQuery.dequeue(this, type);
                }
            });
        }

    });

    // Animations created synchronously will run synchronously
    function createFxNow() {
        setTimeout(clearFxNow, 0);
        return (fxNow = jQuery.now());
    }

    function clearFxNow() {
        fxNow = undefined;
    }

    // Generate parameters to create a standard animation
    function genFx(type, num) {
        var obj = {};

        jQuery.each(fxAttrs.concat.apply([], fxAttrs.slice(0, num)), function () {
            obj[this] = type;
        });

        return obj;
    }

    // Generate shortcuts for custom animations
    jQuery.each({
        slideDown: genFx("show", 1),
        slideUp: genFx("hide", 1),
        slideToggle: genFx("toggle", 1),
        fadeIn: { opacity: "show" },
        fadeOut: { opacity: "hide" },
        fadeToggle: { opacity: "toggle" }
    }, function (name, props) {
        jQuery.fn[name] = function (speed, easing, callback) {
            return this.animate(props, speed, easing, callback);
        };
    });

    jQuery.extend({
        speed: function (speed, easing, fn) {
            var opt = speed && typeof speed === "object" ? jQuery.extend({}, speed) : {
                complete: fn || !fn && easing ||
				jQuery.isFunction(speed) && speed,
                duration: speed,
                easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
            };

            opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
			opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[opt.duration] : jQuery.fx.speeds._default;

            // normalize opt.queue - true/undefined/null -> "fx"
            if (opt.queue == null || opt.queue === true) {
                opt.queue = "fx";
            }

            // Queueing
            opt.old = opt.complete;

            opt.complete = function (noUnmark) {
                if (jQuery.isFunction(opt.old)) {
                    opt.old.call(this);
                }

                if (opt.queue) {
                    jQuery.dequeue(this, opt.queue);
                } else if (noUnmark !== false) {
                    jQuery._unmark(this);
                }
            };

            return opt;
        },

        easing: {
            linear: function (p, n, firstNum, diff) {
                return firstNum + diff * p;
            },
            swing: function (p, n, firstNum, diff) {
                return ((-Math.cos(p * Math.PI) / 2) + 0.5) * diff + firstNum;
            }
        },

        timers: [],

        fx: function (elem, options, prop) {
            this.options = options;
            this.elem = elem;
            this.prop = prop;

            options.orig = options.orig || {};
        }

    });

    jQuery.fx.prototype = {
        // Simple function for setting a style value
        update: function () {
            if (this.options.step) {
                this.options.step.call(this.elem, this.now, this);
            }

            (jQuery.fx.step[this.prop] || jQuery.fx.step._default)(this);
        },

        // Get the current size
        cur: function () {
            if (this.elem[this.prop] != null && (!this.elem.style || this.elem.style[this.prop] == null)) {
                return this.elem[this.prop];
            }

            var parsed,
			r = jQuery.css(this.elem, this.prop);
            // Empty strings, null, undefined and "auto" are converted to 0,
            // complex values such as "rotate(1rad)" are returned as is,
            // simple values such as "10px" are parsed to Float.
            return isNaN(parsed = parseFloat(r)) ? !r || r === "auto" ? 0 : r : parsed;
        },

        // Start an animation from one number to another
        custom: function (from, to, unit) {
            var self = this,
			fx = jQuery.fx;

            this.startTime = fxNow || createFxNow();
            this.end = to;
            this.now = this.start = from;
            this.pos = this.state = 0;
            this.unit = unit || this.unit || (jQuery.cssNumber[this.prop] ? "" : "px");

            function t(gotoEnd) {
                return self.step(gotoEnd);
            }

            t.queue = this.options.queue;
            t.elem = this.elem;
            t.saveState = function () {
                if (self.options.hide && jQuery._data(self.elem, "fxshow" + self.prop) === undefined) {
                    jQuery._data(self.elem, "fxshow" + self.prop, self.start);
                }
            };

            if (t() && jQuery.timers.push(t) && !timerId) {
                timerId = setInterval(fx.tick, fx.interval);
            }
        },

        // Simple 'show' function
        show: function () {
            var dataShow = jQuery._data(this.elem, "fxshow" + this.prop);

            // Remember where we started, so that we can go back to it later
            this.options.orig[this.prop] = dataShow || jQuery.style(this.elem, this.prop);
            this.options.show = true;

            // Begin the animation
            // Make sure that we start at a small width/height to avoid any flash of content
            if (dataShow !== undefined) {
                // This show is picking up where a previous hide or show left off
                this.custom(this.cur(), dataShow);
            } else {
                this.custom(this.prop === "width" || this.prop === "height" ? 1 : 0, this.cur());
            }

            // Start by showing the element
            jQuery(this.elem).show();
        },

        // Simple 'hide' function
        hide: function () {
            // Remember where we started, so that we can go back to it later
            this.options.orig[this.prop] = jQuery._data(this.elem, "fxshow" + this.prop) || jQuery.style(this.elem, this.prop);
            this.options.hide = true;

            // Begin the animation
            this.custom(this.cur(), 0);
        },

        // Each step of an animation
        step: function (gotoEnd) {
            var p, n, complete,
			t = fxNow || createFxNow(),
			done = true,
			elem = this.elem,
			options = this.options;

            if (gotoEnd || t >= options.duration + this.startTime) {
                this.now = this.end;
                this.pos = this.state = 1;
                this.update();

                options.animatedProperties[this.prop] = true;

                for (p in options.animatedProperties) {
                    if (options.animatedProperties[p] !== true) {
                        done = false;
                    }
                }

                if (done) {
                    // Reset the overflow
                    if (options.overflow != null && !jQuery.support.shrinkWrapBlocks) {

                        jQuery.each(["", "X", "Y"], function (index, value) {
                            elem.style["overflow" + value] = options.overflow[index];
                        });
                    }

                    // Hide the element if the "hide" operation was done
                    if (options.hide) {
                        jQuery(elem).hide();
                    }

                    // Reset the properties, if the item has been hidden or shown
                    if (options.hide || options.show) {
                        for (p in options.animatedProperties) {
                            jQuery.style(elem, p, options.orig[p]);
                            jQuery.removeData(elem, "fxshow" + p, true);
                            // Toggle data is no longer needed
                            jQuery.removeData(elem, "toggle" + p, true);
                        }
                    }

                    // Execute the complete function
                    // in the event that the complete function throws an exception
                    // we must ensure it won't be called twice. #5684

                    complete = options.complete;
                    if (complete) {

                        options.complete = false;
                        complete.call(elem);
                    }
                }

                return false;

            } else {
                // classical easing cannot be used with an Infinity duration
                if (options.duration == Infinity) {
                    this.now = t;
                } else {
                    n = t - this.startTime;
                    this.state = n / options.duration;

                    // Perform the easing function, defaults to swing
                    this.pos = jQuery.easing[options.animatedProperties[this.prop]](this.state, n, 0, 1, options.duration);
                    this.now = this.start + ((this.end - this.start) * this.pos);
                }
                // Perform the next step of the animation
                this.update();
            }

            return true;
        }
    };

    jQuery.extend(jQuery.fx, {
        tick: function () {
            var timer,
			timers = jQuery.timers,
			i = 0;

            for (; i < timers.length; i++) {
                timer = timers[i];
                // Checks the timer has not already been removed
                if (!timer() && timers[i] === timer) {
                    timers.splice(i--, 1);
                }
            }

            if (!timers.length) {
                jQuery.fx.stop();
            }
        },

        interval: 13,

        stop: function () {
            clearInterval(timerId);
            timerId = null;
        },

        speeds: {
            slow: 600,
            fast: 200,
            // Default speed
            _default: 400
        },

        step: {
            opacity: function (fx) {
                jQuery.style(fx.elem, "opacity", fx.now);
            },

            _default: function (fx) {
                if (fx.elem.style && fx.elem.style[fx.prop] != null) {
                    fx.elem.style[fx.prop] = fx.now + fx.unit;
                } else {
                    fx.elem[fx.prop] = fx.now;
                }
            }
        }
    });

    // Adds width/height step functions
    // Do not set anything below 0
    jQuery.each(["width", "height"], function (i, prop) {
        jQuery.fx.step[prop] = function (fx) {
            jQuery.style(fx.elem, prop, Math.max(0, fx.now) + fx.unit);
        };
    });

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.animated = function (elem) {
            return jQuery.grep(jQuery.timers, function (fn) {
                return elem === fn.elem;
            }).length;
        };
    }

    // Try to restore the default display value of an element
    function defaultDisplay(nodeName) {

        if (!elemdisplay[nodeName]) {

            var body = document.body,
			elem = jQuery("<" + nodeName + ">").appendTo(body),
			display = elem.css("display");
            elem.remove();

            // If the simple way fails,
            // get element's real default display by attaching it to a temp iframe
            if (display === "none" || display === "") {
                // No iframe to use yet, so create it
                if (!iframe) {
                    iframe = document.createElement("iframe");
                    iframe.frameBorder = iframe.width = iframe.height = 0;
                }

                body.appendChild(iframe);

                // Create a cacheable copy of the iframe document on first call.
                // IE and Opera will allow us to reuse the iframeDoc without re-writing the fake HTML
                // document to it; WebKit & Firefox won't allow reusing the iframe document.
                if (!iframeDoc || !iframe.createElement) {
                    iframeDoc = (iframe.contentWindow || iframe.contentDocument).document;
                    iframeDoc.write((document.compatMode === "CSS1Compat" ? "<!doctype html>" : "") + "<html><body>");
                    iframeDoc.close();
                }

                elem = iframeDoc.createElement(nodeName);

                iframeDoc.body.appendChild(elem);

                display = jQuery.css(elem, "display");
                body.removeChild(iframe);
            }

            // Store the correct default display
            elemdisplay[nodeName] = display;
        }

        return elemdisplay[nodeName];
    }




    var rtable = /^t(?:able|d|h)$/i,
	rroot = /^(?:body|html)$/i;

    if ("getBoundingClientRect" in document.documentElement) {
        jQuery.fn.offset = function (options) {
            var elem = this[0], box;

            if (options) {
                return this.each(function (i) {
                    jQuery.offset.setOffset(this, options, i);
                });
            }

            if (!elem || !elem.ownerDocument) {
                return null;
            }

            if (elem === elem.ownerDocument.body) {
                return jQuery.offset.bodyOffset(elem);
            }

            try {
                box = elem.getBoundingClientRect();
            } catch (e) { }

            var doc = elem.ownerDocument,
			docElem = doc.documentElement;

            // Make sure we're not dealing with a disconnected DOM node
            if (!box || !jQuery.contains(docElem, elem)) {
                return box ? { top: box.top, left: box.left} : { top: 0, left: 0 };
            }

            var body = doc.body,
			win = getWindow(doc),
			clientTop = docElem.clientTop || body.clientTop || 0,
			clientLeft = docElem.clientLeft || body.clientLeft || 0,
			scrollTop = win.pageYOffset || jQuery.support.boxModel && docElem.scrollTop || body.scrollTop,
			scrollLeft = win.pageXOffset || jQuery.support.boxModel && docElem.scrollLeft || body.scrollLeft,
			top = box.top + scrollTop - clientTop,
			left = box.left + scrollLeft - clientLeft;

            return { top: top, left: left };
        };

    } else {
        jQuery.fn.offset = function (options) {
            var elem = this[0];

            if (options) {
                return this.each(function (i) {
                    jQuery.offset.setOffset(this, options, i);
                });
            }

            if (!elem || !elem.ownerDocument) {
                return null;
            }

            if (elem === elem.ownerDocument.body) {
                return jQuery.offset.bodyOffset(elem);
            }

            var computedStyle,
			offsetParent = elem.offsetParent,
			prevOffsetParent = elem,
			doc = elem.ownerDocument,
			docElem = doc.documentElement,
			body = doc.body,
			defaultView = doc.defaultView,
			prevComputedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle,
			top = elem.offsetTop,
			left = elem.offsetLeft;

            while ((elem = elem.parentNode) && elem !== body && elem !== docElem) {
                if (jQuery.support.fixedPosition && prevComputedStyle.position === "fixed") {
                    break;
                }

                computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
                top -= elem.scrollTop;
                left -= elem.scrollLeft;

                if (elem === offsetParent) {
                    top += elem.offsetTop;
                    left += elem.offsetLeft;

                    if (jQuery.support.doesNotAddBorder && !(jQuery.support.doesAddBorderForTableAndCells && rtable.test(elem.nodeName))) {
                        top += parseFloat(computedStyle.borderTopWidth) || 0;
                        left += parseFloat(computedStyle.borderLeftWidth) || 0;
                    }

                    prevOffsetParent = offsetParent;
                    offsetParent = elem.offsetParent;
                }

                if (jQuery.support.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible") {
                    top += parseFloat(computedStyle.borderTopWidth) || 0;
                    left += parseFloat(computedStyle.borderLeftWidth) || 0;
                }

                prevComputedStyle = computedStyle;
            }

            if (prevComputedStyle.position === "relative" || prevComputedStyle.position === "static") {
                top += body.offsetTop;
                left += body.offsetLeft;
            }

            if (jQuery.support.fixedPosition && prevComputedStyle.position === "fixed") {
                top += Math.max(docElem.scrollTop, body.scrollTop);
                left += Math.max(docElem.scrollLeft, body.scrollLeft);
            }

            return { top: top, left: left };
        };
    }

    jQuery.offset = {

        bodyOffset: function (body) {
            var top = body.offsetTop,
			left = body.offsetLeft;

            if (jQuery.support.doesNotIncludeMarginInBodyOffset) {
                top += parseFloat(jQuery.css(body, "marginTop")) || 0;
                left += parseFloat(jQuery.css(body, "marginLeft")) || 0;
            }

            return { top: top, left: left };
        },

        setOffset: function (elem, options, i) {
            var position = jQuery.css(elem, "position");

            // set position first, in-case top/left are set even on static elem
            if (position === "static") {
                elem.style.position = "relative";
            }

            var curElem = jQuery(elem),
			curOffset = curElem.offset(),
			curCSSTop = jQuery.css(elem, "top"),
			curCSSLeft = jQuery.css(elem, "left"),
			calculatePosition = (position === "absolute" || position === "fixed") && jQuery.inArray("auto", [curCSSTop, curCSSLeft]) > -1,
			props = {}, curPosition = {}, curTop, curLeft;

            // need to be able to calculate position if either top or left is auto and position is either absolute or fixed
            if (calculatePosition) {
                curPosition = curElem.position();
                curTop = curPosition.top;
                curLeft = curPosition.left;
            } else {
                curTop = parseFloat(curCSSTop) || 0;
                curLeft = parseFloat(curCSSLeft) || 0;
            }

            if (jQuery.isFunction(options)) {
                options = options.call(elem, i, curOffset);
            }

            if (options.top != null) {
                props.top = (options.top - curOffset.top) + curTop;
            }
            if (options.left != null) {
                props.left = (options.left - curOffset.left) + curLeft;
            }

            if ("using" in options) {
                options.using.call(elem, props);
            } else {
                curElem.css(props);
            }
        }
    };


    jQuery.fn.extend({

        position: function () {
            if (!this[0]) {
                return null;
            }

            var elem = this[0],

            // Get *real* offsetParent
		offsetParent = this.offsetParent(),

            // Get correct offsets
		offset = this.offset(),
		parentOffset = rroot.test(offsetParent[0].nodeName) ? { top: 0, left: 0} : offsetParent.offset();

            // Subtract element margins
            // note: when an element has margin: auto the offsetLeft and marginLeft
            // are the same in Safari causing offset.left to incorrectly be 0
            offset.top -= parseFloat(jQuery.css(elem, "marginTop")) || 0;
            offset.left -= parseFloat(jQuery.css(elem, "marginLeft")) || 0;

            // Add offsetParent borders
            parentOffset.top += parseFloat(jQuery.css(offsetParent[0], "borderTopWidth")) || 0;
            parentOffset.left += parseFloat(jQuery.css(offsetParent[0], "borderLeftWidth")) || 0;

            // Subtract the two offsets
            return {
                top: offset.top - parentOffset.top,
                left: offset.left - parentOffset.left
            };
        },

        offsetParent: function () {
            return this.map(function () {
                var offsetParent = this.offsetParent || document.body;
                while (offsetParent && (!rroot.test(offsetParent.nodeName) && jQuery.css(offsetParent, "position") === "static")) {
                    offsetParent = offsetParent.offsetParent;
                }
                return offsetParent;
            });
        }
    });


    // Create scrollLeft and scrollTop methods
    jQuery.each(["Left", "Top"], function (i, name) {
        var method = "scroll" + name;

        jQuery.fn[method] = function (val) {
            var elem, win;

            if (val === undefined) {
                elem = this[0];

                if (!elem) {
                    return null;
                }

                win = getWindow(elem);

                // Return the scroll offset
                return win ? ("pageXOffset" in win) ? win[i ? "pageYOffset" : "pageXOffset"] :
				jQuery.support.boxModel && win.document.documentElement[method] ||
					win.document.body[method] :
				elem[method];
            }

            // Set the scroll offset
            return this.each(function () {
                win = getWindow(this);

                if (win) {
                    win.scrollTo(
					!i ? val : jQuery(win).scrollLeft(),
					 i ? val : jQuery(win).scrollTop()
				);

                } else {
                    this[method] = val;
                }
            });
        };
    });

    function getWindow(elem) {
        return jQuery.isWindow(elem) ?
		elem :
		elem.nodeType === 9 ?
			elem.defaultView || elem.parentWindow :
			false;
    }




    // Create width, height, innerHeight, innerWidth, outerHeight and outerWidth methods
    jQuery.each(["Height", "Width"], function (i, name) {

        var type = name.toLowerCase();

        // innerHeight and innerWidth
        jQuery.fn["inner" + name] = function () {
            var elem = this[0];
            return elem ?
			elem.style ?
			parseFloat(jQuery.css(elem, type, "padding")) :
			this[type]() :
			null;
        };

        // outerHeight and outerWidth
        jQuery.fn["outer" + name] = function (margin) {
            var elem = this[0];
            return elem ?
			elem.style ?
			parseFloat(jQuery.css(elem, type, margin ? "margin" : "border")) :
			this[type]() :
			null;
        };

        jQuery.fn[type] = function (size) {
            // Get window width or height
            var elem = this[0];
            if (!elem) {
                return size == null ? null : this;
            }

            if (jQuery.isFunction(size)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self[type](size.call(this, i, self[type]()));
                });
            }

            if (jQuery.isWindow(elem)) {
                // Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
                // 3rd condition allows Nokia support, as it supports the docElem prop but not CSS1Compat
                var docElemProp = elem.document.documentElement["client" + name],
				body = elem.document.body;
                return elem.document.compatMode === "CSS1Compat" && docElemProp ||
				body && body["client" + name] || docElemProp;

                // Get document width or height
            } else if (elem.nodeType === 9) {
                // Either scroll[Width/Height] or offset[Width/Height], whichever is greater
                return Math.max(
				elem.documentElement["client" + name],
				elem.body["scroll" + name], elem.documentElement["scroll" + name],
				elem.body["offset" + name], elem.documentElement["offset" + name]
			);

                // Get or set width or height on the element
            } else if (size === undefined) {
                var orig = jQuery.css(elem, type),
				ret = parseFloat(orig);

                return jQuery.isNumeric(ret) ? ret : orig;

                // Set the width or height on the element (default to pixels if value is unitless)
            } else {
                return this.css(type, typeof size === "string" ? size : size + "px");
            }
        };

    });




    // Expose jQuery to the global object
    window.jQuery = window.$ = jQuery;

    // Expose jQuery as an AMD module, but only for AMD loaders that
    // understand the issues with loading multiple versions of jQuery
    // in a page that all might call define(). The loader will indicate
    // they have special allowances for multiple jQuery versions by
    // specifying define.amd.jQuery = true. Register as a named module,
    // since jQuery can be concatenated with other files that may use define,
    // but not use a proper concatenation script that understands anonymous
    // AMD modules. A named AMD is safest and most robust way to register.
    // Lowercase jquery is used because AMD module names are derived from
    // file names, and jQuery is normally delivered in a lowercase file name.
    // Do this after creating the global so that if an AMD module wants to call
    // noConflict to hide this version of jQuery, it will work.
    if (typeof define === "function" && define.amd && define.amd.jQuery) {
        define("jquery", [], function () { return jQuery; });
    }



})(window);

/*
http://www.JSON.org/json2.js
2011-10-19

Public Domain.

NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.

See http://www.JSON.org/js.html


This code should be minified before deployment.
See http://javascript.crockford.com/jsmin.html

USE YOUR OWN COPY. IT IS EXTREMELY UNWISE TO LOAD CODE FROM SERVERS YOU DO
NOT CONTROL.


This file creates a global JSON object containing two methods: stringify
and parse.

JSON.stringify(value, replacer, space)
value       any JavaScript value, usually an object or array.

replacer    an optional parameter that determines how object
values are stringified for objects. It can be a
function or an array of strings.

space       an optional parameter that specifies the indentation
of nested structures. If it is omitted, the text will
be packed without extra whitespace. If it is a number,
it will specify the number of spaces to indent at each
level. If it is a string (such as '\t' or '&nbsp;'),
it contains the characters used to indent at each level.

This method produces a JSON text from a JavaScript value.

When an object value is found, if the object contains a toJSON
method, its toJSON method will be called and the result will be
stringified. A toJSON method does not serialize: it returns the
value represented by the name/value pair that should be serialized,
or undefined if nothing should be serialized. The toJSON method
will be passed the key associated with the value, and this will be
bound to the value

For example, this would serialize Dates as ISO strings.

Date.prototype.toJSON = function (key) {
function f(n) {
// Format integers to have at least two digits.
return n < 10 ? '0' + n : n;
}

return this.getUTCFullYear()   + '-' +
f(this.getUTCMonth() + 1) + '-' +
f(this.getUTCDate())      + 'T' +
f(this.getUTCHours())     + ':' +
f(this.getUTCMinutes())   + ':' +
f(this.getUTCSeconds())   + 'Z';
};

You can provide an optional replacer method. It will be passed the
key and value of each member, with this bound to the containing
object. The value that is returned from your method will be
serialized. If your method returns undefined, then the member will
be excluded from the serialization.

If the replacer parameter is an array of strings, then it will be
used to select the members to be serialized. It filters the results
such that only members with keys listed in the replacer array are
stringified.

Values that do not have JSON representations, such as undefined or
functions, will not be serialized. Such values in objects will be
dropped; in arrays they will be replaced with null. You can use
a replacer function to replace those with JSON values.
JSON.stringify(undefined) returns undefined.

The optional space parameter produces a stringification of the
value that is filled with line breaks and indentation to make it
easier to read.

If the space parameter is a non-empty string, then that string will
be used for indentation. If the space parameter is a number, then
the indentation will be that many spaces.

Example:

text = JSON.stringify(['e', {pluribus: 'unum'}]);
// text is '["e",{"pluribus":"unum"}]'


text = JSON.stringify(['e', {pluribus: 'unum'}], null, '\t');
// text is '[\n\t"e",\n\t{\n\t\t"pluribus": "unum"\n\t}\n]'

text = JSON.stringify([new Date()], function (key, value) {
return this[key] instanceof Date ?
'Date(' + this[key] + ')' : value;
});
// text is '["Date(---current time---)"]'


JSON.parse(text, reviver)
This method parses a JSON text to produce an object or array.
It can throw a SyntaxError exception.

The optional reviver parameter is a function that can filter and
transform the results. It receives each of the keys and values,
and its return value is used instead of the original value.
If it returns what it received, then the structure is not modified.
If it returns undefined then the member is deleted.

Example:

// Parse the text. Values that look like ISO date strings will
// be converted to Date objects.

myData = JSON.parse(text, function (key, value) {
var a;
if (typeof value === 'string') {
a =
/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
if (a) {
return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
+a[5], +a[6]));
}
}
return value;
});

myData = JSON.parse('["Date(09/09/2001)"]', function (key, value) {
var d;
if (typeof value === 'string' &&
value.slice(0, 5) === 'Date(' &&
value.slice(-1) === ')') {
d = new Date(value.slice(5, -1));
if (d) {
return d;
}
}
return value;
});


This is a reference implementation. You are free to copy, modify, or
redistribute.
*/

/*jslint evil: true, regexp: true */

/*members "", "\b", "\t", "\n", "\f", "\r", "\"", JSON, "\\", apply,
call, charCodeAt, getUTCDate, getUTCFullYear, getUTCHours,
getUTCMinutes, getUTCMonth, getUTCSeconds, hasOwnProperty, join,
lastIndex, length, parse, prototype, push, replace, slice, stringify,
test, toJSON, toString, valueOf
*/


// Create a JSON object only if one does not already exist. We create the
// methods in a closure to avoid creating global variables.

var JSON;
if (!JSON) {
    JSON = {};
}

(function () {
    'use strict';

    function f(n) {
        // Format integers to have at least two digits.
        return n < 10 ? '0' + n : n;
    }

    if (typeof Date.prototype.toJSON !== 'function') {

        Date.prototype.toJSON = function (key) {

            return isFinite(this.valueOf())
                ? this.getUTCFullYear() + '-' +
                    f(this.getUTCMonth() + 1) + '-' +
                    f(this.getUTCDate()) + 'T' +
                    f(this.getUTCHours()) + ':' +
                    f(this.getUTCMinutes()) + ':' +
                    f(this.getUTCSeconds()) + 'Z'
                : null;
        };

        String.prototype.toJSON =
            Number.prototype.toJSON =
            Boolean.prototype.toJSON = function (key) {
                return this.valueOf();
            };
    }

    var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;


    function quote(string) {

        // If the string contains no control characters, no quote characters, and no
        // backslash characters, then we can safely slap some quotes around it.
        // Otherwise we must also replace the offending characters with safe escape
        // sequences.

        escapable.lastIndex = 0;
        return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
            var c = meta[a];
            return typeof c === 'string'
                ? c
                : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
        }) + '"' : '"' + string + '"';
    }


    function str(key, holder) {

        // Produce a string from holder[key].

        var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

        // If the value has a toJSON method, call it to obtain a replacement value.

        if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
            value = value.toJSON(key);
        }

        // If we were called with a replacer function, then call the replacer to
        // obtain a replacement value.

        if (typeof rep === 'function') {
            value = rep.call(holder, key, value);
        }

        // What happens next depends on the value's type.

        switch (typeof value) {
            case 'string':
                return quote(value);

            case 'number':

                // JSON numbers must be finite. Encode non-finite numbers as null.

                return isFinite(value) ? String(value) : 'null';

            case 'boolean':
            case 'null':

                // If the value is a boolean or null, convert it to a string. Note:
                // typeof null does not produce 'null'. The case is included here in
                // the remote chance that this gets fixed someday.

                return String(value);

                // If the type is 'object', we might be dealing with an object or an array or
                // null.

            case 'object':

                // Due to a specification blunder in ECMAScript, typeof null is 'object',
                // so watch out for that case.

                if (!value) {
                    return 'null';
                }

                // Make an array to hold the partial results of stringifying this object value.

                gap += indent;
                partial = [];

                // Is the value an array?

                if (Object.prototype.toString.apply(value) === '[object Array]') {

                    // The value is an array. Stringify every element. Use null as a placeholder
                    // for non-JSON values.

                    length = value.length;
                    for (i = 0; i < length; i += 1) {
                        partial[i] = str(i, value) || 'null';
                    }

                    // Join all of the elements together, separated with commas, and wrap them in
                    // brackets.

                    v = partial.length === 0
                    ? '[]'
                    : gap
                    ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']'
                    : '[' + partial.join(',') + ']';
                    gap = mind;
                    return v;
                }

                // If the replacer is an array, use it to select the members to be stringified.

                if (rep && typeof rep === 'object') {
                    length = rep.length;
                    for (i = 0; i < length; i += 1) {
                        if (typeof rep[i] === 'string') {
                            k = rep[i];
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                } else {

                    // Otherwise, iterate through all of the keys in the object.

                    for (k in value) {
                        if (Object.prototype.hasOwnProperty.call(value, k)) {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                }

                // Join all of the member texts together, separated with commas,
                // and wrap them in braces.

                v = partial.length === 0
                ? '{}'
                : gap
                ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}'
                : '{' + partial.join(',') + '}';
                gap = mind;
                return v;
        }
    }

    // If the JSON object does not yet have a stringify method, give it one.

    if (typeof JSON.stringify !== 'function') {
        JSON.stringify = function (value, replacer, space) {

            // The stringify method takes a value and an optional replacer, and an optional
            // space parameter, and returns a JSON text. The replacer can be a function
            // that can replace values, or an array of strings that will select the keys.
            // A default replacer method can be provided. Use of the space parameter can
            // produce text that is more easily readable.

            var i;
            gap = '';
            indent = '';

            // If the space parameter is a number, make an indent string containing that
            // many spaces.

            if (typeof space === 'number') {
                for (i = 0; i < space; i += 1) {
                    indent += ' ';
                }

                // If the space parameter is a string, it will be used as the indent string.

            } else if (typeof space === 'string') {
                indent = space;
            }

            // If there is a replacer, it must be a function or an array.
            // Otherwise, throw an error.

            rep = replacer;
            if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                    typeof replacer.length !== 'number')) {
                throw new Error('JSON.stringify');
            }

            // Make a fake root object containing our value under the key of ''.
            // Return the result of stringifying the value.

            return str('', { '': value });
        };
    }

    $.toJSON = JSON.stringify ;

    // If the JSON object does not yet have a parse method, give it one.
    //UDI , 用 JSON2 的方法替换 系统方法,保持一致性.因为 系统的转换方法不能对时间格式进行转换.
    //if (typeof JSON.parse !== 'function') {
        JSON.parse = function (text, reviver) {

            // The parse method takes a text and an optional reviver function, and returns
            // a JavaScript value if the text is a valid JSON text.

            var j;

            function walk(holder, key) {

                // The walk method is used to recursively walk the resulting structure so
                // that modifications can be made.

                var k, v, value = holder[key];
                if (value && typeof value === 'object') {
                    for (k in value) {
                        if (Object.prototype.hasOwnProperty.call(value, k)) {
                            v = walk(value, k);
                            if (v !== undefined) {
                                value[k] = v;
                            } else {
                                delete value[k];
                            }
                        }
                    }
                }
                return reviver.call(holder, key, value);
            }

           
                    // *** RAS Update: RegEx handler for dates ISO and MS AJAX style
      function  regExDate (str,p1, p2,offset,s) 
	    {	
            str = str.substring(1).replace('"','');
            var date = str;
        
            // MS Ajax date: /Date(19834141)/
            if (/\/Date(.*)\//.test(str)) {        
                str = str.match(/Date\((.*?)\)/)[1];                        
                date = "new Date(" +  parseInt(str) + ")";
            }
            else { // ISO Date 2007-12-31T23:59:59Z                                     
                var matches = str.split( /[-,:,T,Z]/);        
                matches[1] = (parseInt(matches[1],0)-1).toString();                     
                date = "new Date(Date.UTC(" + matches.join(",") + "))";         
           }                  
            return date;
        }
    

            // Parsing happens in four stages. In the first stage, we replace certain
            // Unicode characters with escape sequences. JavaScript handles many characters
            // incorrectly, either silently deleting them, or treating them as line endings.

            text = String(text);
            cx.lastIndex = 0;
            if (cx.test(text)) {
                text = text.replace(cx, function (a) {
                    return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                });
            }

            // In the second stage, we run the text against regular expressions that look
            // for non-JSON patterns. We are especially concerned with '()' and 'new'
            // because they can cause invocation, and '=' because it can cause mutation.
            // But just to be safe, we want to reject all unexpected forms.

            // We split the second stage into 4 regexp operations in order to work around
            // crippling inefficiencies in IE's and Safari's regexp engines. First we
            // replace the JSON backslash pairs with '@' (a non-JSON character). Second, we
            // replace all simple value tokens with ']' characters. Third, we delete all
            // open brackets that follow a colon or comma or that begin the text. Finally,
            // we look to see that the remaining characters are only whitespace or ']' or
            // ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

            if (/^[\],:{}\s]*$/
                    .test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                        .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                        .replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                // In the third stage we use the eval function to compile the text into a
                // JavaScript structure. The '{' operator is subject to a syntactic ambiguity
                // in JavaScript: it can begin a block or an object literal. We wrap the text
                // in parens to eliminate the ambiguity.


        // *** RAS Update:  Fix up Dates: ISO and MS AJAX format support
        //var regEx = /(\"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}.*?\")|(\"\\*\/Date\(.*?\)\\*\/")/g;
        var regEx = /(\"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}.*?\")|(\"\\*\/Date\(.*?\)\\*\/")/g;
        text = text.replace(regEx, regExDate);
        // *** End RAS Update


                j = eval('(' + text + ')');

                // In the optional fourth stage, we recursively walk the new structure, passing
                // each name/value pair to a reviver function for possible transformation.

                return typeof reviver === 'function'
                    ? walk({ '': j }, '')
                    : j;
            }

            // If the text is not JSON parseable, then a SyntaxError is thrown.

            throw new SyntaxError('JSON.parse');
        };

//    } //UDI , 去除 if (typeof JSON.parse !== 'function') 
} ());

/*
$.cookie(’the_cookie’, ‘the_value’);
设置cookie的值
example $.cookie(’the_cookie’, ‘the_value’, {expires: 7, path: ‘/’, domain: ‘jquery.com’, secure: true});
新建一个cookie 包括有效期 路径 域名等
example $.cookie(’the_cookie’, ‘the_value’);
新建cookie
example $.cookie(’the_cookie’, null);
删除一个cookie
*/
jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        var path = options.path ? '; path=' + options.path : '/';
        var domain = options.domain ? '; domain=' + options.domain : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

/**  
*  
* timer() provides a cleaner way to handle intervals    
*  
*     @usage  
* $.timer(interval, callback);  
*  
*  
* @example  
* $.timer(1000, function (timer) {  
*     alert("hello");  
*     timer.stop();  
* });  
* @desc Show an alert box after 1 second and stop  
*   
* @example  
* var second = false;  
*     $.timer(1000, function (timer) {  
*             if (!second) {  
*                     alert('First time!');  
*                     second = true;  
*                     timer.reset(3000);  
*             }  
*             else {  
*                     alert('Second time');  
*                     timer.stop();  
*             }  
*     });  
* @desc Show an alert box after 1 second and show another after 3 seconds  
*  
*   
*/
jQuery.timer = function (interval, callback) {
    var interval = interval || 100;
    if (!callback)
        return false;

    var doer = jv.getDoer();
    _timer = function (interval, callback) {
        var self = this;
        this.stop = function () {
            clearInterval(self.id);
        };

        this.internalCallback = function () {
            callback(self, { originalEvent: true, target: doer });
        };

        this.reset = function (val) {
            if (self.id)
                clearInterval(self.id);

            var val = val || 100;
            this.id = setInterval(this.internalCallback, val);
        };

        this.interval = interval;
        this.id = setInterval(this.internalCallback, this.interval);

        return this.id;
    };

    return new _timer(interval, callback);
};

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
window.Alert = window.alert;
window.Confirm = window.confirm;

//扩展到 JQuery
(function ($) {
    //本页标志性含数 $.fn.LoadView
    if ($.fn.LoadView) return;

    $(window).load(function () {
        jv.LoadJsCss("js", jv.Root + "Res/My97/wdatepicker.js");
        jv.MyOnLoad();
    });

    if ($.browser.msie) {
        var unloadEvent = function () {


            //http://fogtower.iteye.com/blog/617997
            $("form,button,input,select,textarea,a,img").unbind().removeData();
            //,table,tr,td
            //添加这一句，将减少大部分内存泄露
            $.cache = null;
            CollectGarbage();

            window.detachEvent('onunload', unloadEvent);



            //https://github.com/mootools/mootools-core/issues/2329
            //http://com.hemiola.com/2009/11/23/memory-leaks-in-ie8/
            //http://blog.csdn.net/guo_rui22/article/details/3165320

        };

        window.attachEvent("onunload", unloadEvent);
    }


    //是否定义了某个事件, 所有的对象是否全部都定义了该事件. 返回最后一个对象定义的事件。
    $.fn.IsBindEvent = $.fn.isBindEvent = function (eventName) {
        var found = false;
        this.each(function () {
            if (this["on" + eventName]) {
                found = this["on" + eventName];
            }
            else {
                var self = $(this);
                found = self.data("events") && self.data("events")[eventName];
            }
            return found;
        });

        return found;
    };

    //offset 的另一实现,请参考: http://kb.cnblogs.com/a/1710726/
    //$.fn.Offset = $.fn.offset = function (options) {
    //    var elem = this[0];
    //    if (options) {
    //        return this.each(function (i) {
    //            jQuery.offset.setOffset(this, options, i);
    //        });
    //    }
    //    if (!elem || !elem.ownerDocument) {
    //        return null;
    //    }

    //    //特殊处理body  
    //    if (elem === elem.ownerDocument.body) {
    //        return jQuery.offset.bodyOffset(elem);
    //    }

    //    //获得关于offset相关的浏览器特征  
    //    jQuery.offset.initialize();

    //    var offsetParent = elem.offsetParent, prevOffsetParent = elem,
    //    doc = elem.ownerDocument, computedStyle, docElem = doc.documentElement,
    //    body = doc.body, defaultView = doc.defaultView,
    //    prevComputedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle,
    //    top = elem.offsetTop, left = elem.offsetLeft;

    //    while ((elem = elem.parentNode) && elem !== body && elem !== docElem) {
    //        //HTML，BODY，以及不具备CSS盒子模型的元素及display为fixed的元素没有offsetParent属性  
    //        if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
    //            break;
    //        }
    //        computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
    //        top -= elem.scrollTop;
    //        left -= elem.scrollLeft;
    //        if (elem === offsetParent) {
    //            top += elem.offsetTop;
    //            left += elem.offsetLeft;

    //            //offset应该返回的是border-box，但在一些表格元素却没有计算它们的border值，需要自行添加  
    //            //在IE下表格元素的display为table,table-row与table-cell  
    //            if (jQuery.offset.doesNotAddBorder && !(jQuery.offset.doesAddBorderForTableAndCells && /^t(able|d|h)$/i.test(elem.nodeName))) {
    //                top += parseFloat(computedStyle.borderTopWidth) || 0;
    //                left += parseFloat(computedStyle.borderLeftWidth) || 0;
    //            }
    //            prevOffsetParent = offsetParent, offsetParent = elem.offsetParent;
    //        }

    //        //修正safari的错误  
    //        if (jQuery.offset.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible") {
    //            top += parseFloat(computedStyle.borderTopWidth) || 0;
    //            left += parseFloat(computedStyle.borderLeftWidth) || 0;
    //        }
    //        prevComputedStyle = computedStyle;
    //    }

    //    //最后加上body的偏移量  
    //    if (prevComputedStyle.position === "relative" || prevComputedStyle.position === "static") {
    //        top += body.offsetTop;
    //        left += body.offsetLeft;
    //    }

    //    //使用固定定位，可能出现滚动条，我们要获得取大的滚动距离  
    //    if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
    //        top += Math.max(docElem.scrollTop, body.scrollTop);
    //        left += Math.max(docElem.scrollLeft, body.scrollLeft);
    //    }

    //    return { top: top, left: left };
    //};


    //模拟 lambda.select
    $.fn.out = function (callback) {
        var newObj = [];
        if (callback) {
            this.each(function (_i, _d) { newObj.push(callback(_i, _d)); });
            return $(newObj);
        }
        else return this;
    };
    //转到 Jquery，加入 jv.boxdy 范围。如果页面同时有 id,name ,  则按name . 
    //prefix 前缀，约定： 如果为 true ，表示自动截取 $ _ 最后部分。 否则不截取，默认为 空字符串 表示不截取。
    $.fn.GetDivJson = $.fn.getDivJson = function (prefix, option) {
        if ($.isPlainObject(prefix) && !option) {
            option = prefix;
            prefix = "";
        }
        if (prefix !== true) prefix = prefix || "";

        var p = $.extend({
            valueProc: function (val) { return (val || "").trim(); },
            keyProc: function (key) {
                if (!key) return key;
                if (prefix === true) {
                    return key.slice(Math.max(key.lastIndexOf("$"), key.lastIndexOf("_")) + 1);
                }
                else {
                    if (!prefix) return key;
                    if (key.indexOf(prefix) != 0) return "";
                    return key.slice(prefix.length);
                }
            }
        }, option);

        //        var idValues = {}, nameValues = {};
        var jsonVal = {};

        //keyFunc 表示有key 值 存在时key的回调， valFunc 表示有key值存在时val的回调
        var addVal = function (key, val, keyFunc, valFunc) {
            if (jsonVal[key]) {
                var ary = jsonVal[key].mySplit(",", true);
                ary.push(val);
                jsonVal[key] = ary.join(",");
            }
            else {
                jsonVal[key] = val;
            }
        };


        var setJsonValue = function (inps) {

            for (var i = 0, length = inps.length; i < length; i++) {
                var c = inps[i];
                var nameKey = p.keyProc(c.name || c.id || c.getAttribute("name"));
                if (!nameKey) continue;

                if (c.tagName == "SELECT") {
                    if (c.multiple) {
                        var selCount = 0;
                        for (var j = 0, ol = c.options.length; j < ol; j++) {
                            var op = c.options[j];
                            if (op.selected) {
                                addVal(nameKey + "[" + (selCount++) + "]", p.valueProc(op.value));
                            }
                        }
                    }
                    else {
                        jsonVal[nameKey] = p.valueProc(c.value);
                    }
                    continue;
                }
                else if (c.tagName == "INPUT") {
                    if (c["type"] == "checkbox") {
                        var checkValues = {};

                        if (c.checked) {
                            var chkCount = 0;
                            for (var k in jsonVal) {
                                if (k.indexOf(nameKey + "[") == 0) { chkCount++; }
                            }
                            addVal(nameKey + "[" + chkCount + "]", p.valueProc(c.value));
                        }
                        continue;
                    }
                    else if (c["type"] == "radio") {
                        if (c.checked) {
                            jsonVal[nameKey] = p.valueProc(c.value);
                        }
                        continue;
                    }

                    //在 .MyDateTime 时，要使用 $.fn.val 方法获取下拉列表里的值。
                    var jc = $(c);
                    jsonVal[nameKey] = p.valueProc(jc.val());
                    continue;
                }
                else if ("BUTTON" == c.tagName) {
                    jsonVal[nameKey] = p.valueProc(c.value);
                }
                else if ("TEXTAREA" == c.tagName) {
                    if ((" " + c.className + " ").indexOf(" tinymce ") >= 0) {
                        //tinymce 3.x 需要通过　jQuery的　val 取值。
                        jsonVal[nameKey] = $(c).val();
                    } else {
                        jsonVal[nameKey] = p.valueProc(c.value);
                    }
                }
                else if (jv.isLeaf(c)) {
                    jsonVal[nameKey] = p.valueProc(c.innerHTML);
                }
            }
        };


        this.each(function () {
            var inps = jv.findInput(this);
            if (p.extDom && p.extDom) {
                var extinps = p.extDom();
                for (var i = 0; i < extinps.length ; i++) {
                    inps.push(extinps[i]);
                }
            }

            var idInputs = [], nameInputs = [];

            $.each(inps, function (i, d) {
                if (d.id) { idInputs.push(d); }
                else nameInputs.push(d);
            });


            setJsonValue(idInputs);
            setJsonValue(nameInputs);
        });

        return jsonVal;
    };



    //prefix 前缀，约定： 如果为 true ，表示自动截取 $ _ 最后部分。 否则不截取，默认为 空字符串 表示不截取。
    $.fn.SetDivJson = $.fn.setDivJson = function (prefix, JsonValue, option) {
        if ($.isPlainObject(prefix) && !option) {
            option = (JsonValue || null);
            JsonValue = prefix;
            prefix = "";
        }

        if (prefix !== true) prefix = prefix || "";
        var container = this;
        if (container.length == 0) { return; }
        var p = $.extend({
            refMark: "::",   //1::张三 ,表示设置Id为1，显示的名字是张三。
            clearChecked: true,
            findCallback: function (name, val) {
                if ((name.slice(0, 1) == "#") || (name.slice(0, 2) == "\\#")) {
                    if (name.slice(0, 1) == "#") name = name.slice(1);
                    else name = name.slice(2);

                    if (prefix === true) {
                        return $("#" + name, container)[0] || $("[id$=_" + name + "]", container)[0];
                    }
                    else return $("#" + prefix + name, container)[0];
                }
                if (prefix === true) {
                    //优先使用没有前缀的。

                    var jObj;
                    jObj = $("[name=" + name + "]", container);
                    if (jObj.length) return jObj[0];

                    jObj = $("#" + name, container);
                    if (jObj.length) return jObj[0];

                    jObj = $("[name$=_" + name + "]", container);
                    if (jObj.length) return jObj[0];

                    return $("[id$=_" + name + "]", container)[0];
                }
                else {
                    var cons = $("[name=" + prefix + name + "]", container[0]);//container[0].ownerDocument.getElementsByName(prefix + name);
                    if (cons.length > 1) {
                        if (jv.IsNull(val) == false) {
                            //查找某个value值的控件
                            for (var i = 0, length = cons.length; i < length; i++) {
                                var n = cons[i];
                                if (n.value == val && jv.inContainer(n, container[0])) return n;
                            }
                        }
                    }
                    if (cons && cons.length && jv.inContainer(cons[0], container[0])) return cons[0];

                    cons = $("#" + prefix + name, container[0]);//container[0].ownerDocument.getElementById(prefix + name);
                    if (jv.inContainer(cons, container[0])) return cons;
                    return null;
                }
            }
        }, option);

        var setValue = function (cobj, value) {
            var tagName = cobj.tagName;
            if (tagName == "INPUT") {
                var type = cobj["type"];
                if (type == "checkbox" || (type == "radio")) {
                    cobj.checked = true;
                    return;
                }
                else {
                    var self = $(cobj);
                    if (value == "0001/01/01 00:00" || value == "0001/1/1 0:00:00") {
                        self.val("");
                    }
                    else {
                        //对于 .MyDateTime 来说，要使用 $.fn.val 方法进行设置。
                        self.val(value);
                    }
                    return;
                }
            }
            else if (tagName == "OPTION") {
                cobj.selected = true;
                return;
            }
            else if (tagName == "TEXTAREA") {
                cobj.value = value;
                return;
            }
            else if (["SELECT", "BUTTON"].indexOf(tagName) >= 0) {
                cobj.value = value;
            }
            else if (jv.isLeaf(cobj)) {
                cobj.innerHTML = value;
            }
        };

        //清除选中状态。
        if (p.clearChecked) {
            {
                var chks = container[0].getElementsByTagName("input");
                for (var i = 0, length = chks.length; i < length; i++) {
                    var n = chks[i];

                    if (n.checked) {
                        n.checked = false;
                    }
                }
            }
            {
                var chks = container[0].getElementsByTagName("select");
                for (var i = 0, length = chks.length; i < length; i++) {
                    var n = chks[i];

                    n.selectedIndex = -1;
                }
            }
        }


        for (var jkey in JsonValue) {
            //把 $ 替换成 \$ ； 把 # 替换成 \# , 因为它们在 jQuery 选择器中表示特殊的含义 。

            var val = JsonValue[jkey];

            var key = jkey.replace(/\$/g, "\\$").replace(/\#/g, "\\#");

            var leftKIndex = key.indexOf('['), rightKIndex = key.indexOf(']'),
                isCheck = (key.slice(-1) == ']' && (leftKIndex < rightKIndex));
            var checkIndex = -1;
            if (isCheck) {
                checkIndex = parseInt(key.slice(leftKIndex + 1, rightKIndex));
                key = key.slice(0, leftKIndex);
            }

            var cobj = p.findCallback(key, isCheck ? val : null);

            if (cobj) {
                if (prefix === true && val.indexOf("::") > 0) {
                    if ($(cobj).is(":hidden")) {
                        var sect = val.split("::");
                        setValue(cobj, sect[0]);

                        //查找 name
                        jv.SetDisplayValue({ obj: cobj }, null, [sect[1]]);
                    }
                }
                else {
                    setValue(cobj, val);
                }
            }
        }
        return this;
    };

    // 工具栏中的  .view 不会被去除.
    $.fn.SetDetail = $.fn.setDetail = function (options) {
        var p = $.extend({ callback: false, detail: "Detail", container: jv.boxdy() }, options);

        //弹出页面没有导航栏.
        if (jv.IsInBoxy()) {
            $(".Navigate", p.contaier).hide();
        }



        //var procEdit = function () {
        //    jv.PopTextArea($("textarea", p.container));
        //};

        if ((jv.page()["action"] || "").toLowerCase() != p.detail.toLowerCase()) {
            //procEdit();
            return this;
        }


        var _setDetail = function (con) {
            var container = jv.boxdy(con);

            //设置属性和数据.在弹出列表时,方便查找.
            //container
            //    .addClass("jvDetail")
            //    .data("jvDetail", p);
            //处理权限
            //jv.TestRole();        //卡片先不处理权限.

            //            var jcon = $($(".Main:last", jcon)[0] || jcon);



            container.find(":text,textarea").filter(":visible").each(function () {
                var jd = $(this), val = jd.val();

                jd.closest(".val").addClass("Wrap");

                var content = $("<span></span>")
                    .css("height", "100%")
                //                    .css("word-wrap", "break-word")
                //                    .css("white-space", "normal")
                //                    .css("width", jd.width() + parseInt(jd.css("paddingRight")) + +parseInt(jd.css("paddingLeft")))
                    .html(jv.encode(val))
                    .insertAfter(jd)
                ;

                var id = jd.attr("id"), name = jd.attr("name");

                jd.remove();

                var hid = $("<input type='hidden' value='" + val + "' />");

                if (id) hid.attr("id", id);
                if (name) hid.attr("name", name);

                hid.insertAfter(content);
            });

            container.find(":checked").each(function (i) {
                var jd = $(this);
                if (i > 0) {
                    jd.after(" , ");
                }
                jd.remove();
            });

            container.find(":radio,:checkbox").each(function () {
                $("label[for=" + this.id + "]").remove();
                $(this).remove();
            });

            //                container.find(".MyTool").remove();
            container.find(".boxy-inner .BoxyTool").children().not(".view").remove();
            container.find(".PageTool").children().not(".view").remove();


            var myTool = container.find(".MyTool,.myTool");
            myTool.children().not(".view").remove();
            if (!myTool.children().length) { myTool.remove(); }

            container.find(":file").each(function () {
                $(this).remove();
            });

            $(".MyDateDisplay", container).find("span").unbind();


            container.find(".kv .key.Link").unbind("click").removeClass("Link");
            //Boxy.getOne().resize();


            if (p.callback) p.callback();
        };

        this.each(function () {
            _setDetail(this);
        });
    };

    //加 ed ，是因为钩子在最后执行。
    $.fn.Hooked = $.fn.hooked = function (props, func) {
        //props 为空的容错。
        props = props.mySplit(",", true);
        $(props).each(function (i, d) {
            if (d == "" || d == "*") {
                props = props.removeAt(i);
            }
        });
        return this.each(function (i, d) {
            var id = d.id || d.name || d.getAttribute("name"),
            jod = $(d),
            fnc = function (ev, prop) {
                $(props).filter(function () {
                    if (prop == this || prop.indexOf(this + ".") == 0) { func(ev, prop); return false; }
                });
            };

            if (typeof (this.onpropertychange) == "object") {
                jod.bind("propertychange." + id, function (ev) {
                    if (jv.GetDoer(ev) != d) return;
                    fnc(ev, event.propertyName);
                });
            }
            else if ($.browser.mozilla) {
                jod.bind("DOMAttrModified." + id, function (ev) {
                    if (jv.GetDoer(ev) != d) return;
                    var prop = ev.attrName;
                    if (prop == "style") {
                        if (jv.HasValue(ev.newValue)) {
                            prop += "." + ev.newValue.mySplit(":", true)[0];
                        }
                        else if (jv.HasValue(ev.prevValue)) {
                            prop += "." + ev.prevValue.mySplit(":", true)[0];
                        }
                    }
                    fnc(ev, prop);
                });
            }
            else
                //待测其它浏览器。
                return;
        });
    };
    $.fn.OneClick = $.fn.oneClick = function (timeout) {

        this.each(function () {
            if (!this.oned) {
                this.oned = true;

                var jd = $(this);
                jd.data("oriclick", this.onclick);

                var joclick = [];
                if (jd.data("events") && jd.data("events")["click"]) {
                    var clicks = jd.data("events")["click"];
                    for (var i = 0; i < clicks.length; i++) {
                        joclick.push(clicks[i]);
                    }
                }
                jd.data("orijclick", joclick);

                jd.unbind("click");

                this.onclick = function (ev) {
                    var ret, jobj = $(this);
                    jv.SetDisable(jobj, timeout);


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


                };
            }
        });

        return this;
    };

    $.fn.SkinVar = $.fn.skinVar = function (options) {
        //后面的定义会覆盖前面，忽略值为0的值。
        var defaults = {
            Corner: {},
            LangImg: {},
            LangCss: []
        }

        if (!options) {
            options = {};
        }
        options.Corner = $.extend(defaults.Corner, options.Corner);
        options.LangImg = $.extend(defaults.LangImg, options.LangImg);
        options.LangCss = $.extend(defaults.LangCss, options.LangCss);

        this.each(function () {
            //jv.SetCorner(options.Corner);

            for (var p in options.LangImg) {
                $("." + p).attr('src', options.LangImg[p].format(jv.page()["Lang"] || "Zh"));
            }

            $(options.LangCss).each(function () {
                $("." + this.format("")).addClass(this.format(jv.page()["Lang"] || "Zh"))
            });

            if (($(".ListPhoto").attr("src") || "").length == 0) {
                $(".ListPhoto").parents(".Sect:first").hide();
            }
            if (($(".SkinTopBanner").attr("src") || "").length == 0) {
                $(".SkinTopBanner").hide();
            }
            if ($(".deptTitleImg").length > 0) {
                if ($(".deptTitleImg").height() > 100) { $(".deptTitleImg").height(100); }
                if ($(".deptTitleImg").width() > 600) { $(".deptTitleImg").css("height", null); $(".deptTitleImg").width(600); }
            }
        });
    };

    $.fn.LoadView = $.fn.loadView = function (setting) {
        //errror : 出错时的 CallBack , 参数依次表示: Response, 容器. error返回值含义表示是否继续执行默认出错处理.
        var p = $.extend(true, { url: false, postType: "GET", param: { LoadView: true }, filter: false, callback: false, error: false }, setting);

        p.url = jv.url(p.url);

        var theUrl = jv.url(window.location);
        if (theUrl.attr("Html") == "True") {
            p.url.attr("Html", "True");
        }
        else {
            p.url.attr("History", "False"); //强制 History 为False . Udi.
        }

        if (theUrl.url.slice(-5) == ".html") {
            var search = "";
            //转到 HTML
            for (var key in p.url.kv) {
                if (key == "_") continue;
                if (key == "Html") continue;
                if (key == "History") continue;
                search += "." + key + (p.url.kv[key] ? "." + p.url.kv[key] : "");
            }


            p.url.url = p.url.url.slice(0, -5) + search + ".html";
            p.url.search = "";
            p.url.kv = {};
        }

        p.url = p.url.tohref();

        parg = arguments[0];
        parg.originalEvent = true;
        parg.target = this[0];


        var LoadOne = function (_d) {
            var jd = $(_d), timer = $.timer(1000, function (timer) {
                timer.stop();
                if (!jd.attr("class")) { jd.empty(); jd.html("正在加载,请稍等...."); }
            });

            //                    .width(jd.width() + "px")
            //                    .height( jd.height() + "px")
            //                    .addClass("LoadingWillRemove")


            if (jd.hasClass("jvRequest")) {
                jv.UnloadRequest();
            }

            var ajax = {
                url: p.url, type: p.postType, dataType: 'html', cache: false, data: p.param,
                beforeSend: function (xhr) {
                    jv.CreateEventSource(xhr, _d);
                },
                success: function (html) {
                    if (timer) timer.stop();

                    if (html[0] == "{") {
                        var res = null;
                        try
                        { res = $.parseJSON(html); }
                        catch (e) { }
                        if (jv.IsNull(res) == false && res.data == "power") {
                            jd.html(res.msg);
                            return;
                        }
                    }

                    html = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/.exec(html);
                    html = html[1].replace(/<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig, "<$1></$2>");

                    var div = document.createElement("div");
                    div.style.display = "none";
                    document.body.appendChild(div);
                    div.innerHTML = "div<div>" + html + "</div>";

                    html = $(div.lastChild.childNodes);

                    var htmlFilter = jv.myFilter(html, p.filter);
                    var inscript = [];

                    htmlFilter.each(function () {
                        var ins = this.getElementsByTagName("script");
                        for (var i = 0, len = ins.length; i < len; i++) {
                            var s = ins[0];
                            inscript.push(s);
                            s.parentNode.removeChild(s);
                        }
                    });

                    jd.addClass("jvRequest_waite").empty();
                    jd.prepend(htmlFilter);
                    //.find(">.LoadingWillRemove").remove() ;
                    jv.execHtml(html.add(inscript), null, _d);
                    if (p.callback) { p.callback(_d); }

                    document.body.removeChild(div);
                },
                error: function (ev) {
                    if (timer) timer.stop();
                    if (!p.error || (p.error && (p.error(ev, jd) === false))) {
                        var response = $(ev.responseText),
                                msg = response.filter("title").text();
                        if (!msg) {
                            for (var i = 0; i < response.length; i++) {
                                //                            if ( !response[i].text ) continue ;
                                msg = $(response[i]).text();
                                if (!!msg) break;
                            }
                        }

                        var msg = "<div style='color:red'>" +
                                msg +
                                "</div><br />请刷新后再试";

                        jd.html(msg);
                    }
                }
            };
            jQuery.ajax(ajax);
        };

        this
        //.css("position","relative")
            .each(function () {
                LoadOne(this, { originalEvent: true, target: this });
            });
        return this;
    };


    $.fn.Mytip = $.fn.mytip = function (msg, options) {

        var THIS = this,
        p = {
            content: msg,
            //showOn: "focus",
            alignTo: 'target',
            alignX: 'inner-left',
            offsetX: 0,
            offsetY: 5
        };
        p = $.extend(p, options),
        //if ($(obj).data("events") && $(obj).data("events")["focus"] && !$(obj).data("poshytip")) { showType = "hover"; }

        poshytipCallBack = function () {
            THIS.poshytip("destroy").poshytip(p).poshytip(msg ? "show" : "hide");
            var tip = THIS.data("poshytip");
            if (tip) {
                tip.$tip.dblclick(function (a, b) {
                    $(this).hide();
                });
            };
        };

        if (this.poshytip) {
            poshytipCallBack();
        }
        else { jv.MyLoadOn["jv.chk.url"](true, poshytipCallBack); }
    };

    $.fn.CoverDiv = $.fn.coverDiv = function () {
        this.find(">div.cover").remove();
        this.each(function () {
            var $self = $(this);
            var h = $self.height();
            var cover = document.createElement("div");
            cover.setAttribute("class", "cover");

            cover.style.marginBottom = 0 - h + "px";
            cover.style.height = h + "px";

            $self.prepend(cover);
        });

        return this;
    };

    $.fn.ClearCover = $.fn.clearCover = function () {
        this.find(">.cover").remove();
        return this;
    };


    $.fn.UpFile = $.fn.upFile = function (option) {
        var jTHIS = $(this);
        var listFile;
        if (!option) option = {};
        //修复参数：
        if (!option.maxFile && option.max) {
            option.maxFile = option.max;
            delete option.max;
        }


        this.each(function () {

            //更多参数，详见： fileuploader.js 。
            var p = $.extend({
                sizeLimit: 0,
                maxFile: 1,
                data: false, //数据源。[{name:"文件一",id:2,url:"",size:"" }]
                element: this,
                hidden: this.id,
                edit: true,
                multiple: true,
                onSubmit: false,
                onComplete: false,
                listFile: "list"
            }, option);

            var self = $(this);
            var btnText = self.text();
            if (btnText) {
                p.uploadButtonText = btnText;
            }

            //参数
            p.on_Submit = p.onSubmit;
            p.on_Complete = p.onComplete;

            p.onSubmit = function (id, fileFullName) {
                var qqFile = this;
                var fs = fileFullName.mySplit(/\\|\//g, true);
                var name = fs[fs.length - 1].mySplit(".", true)[0];
                var res = name;
                //var res = window.showModalDialog(jv.Root + "html/FileName.html", name, "dialogWidth:380px;dialogHeight:140px;center:yes;resizable:yes;");
                //if (!res) {
                //    this._remove(id);
                //    return false;
                //}

                jv.StartLoading(true);
                qqFile._options.params["FileName"] = res;

                if (p.on_Submit) p.on_Submit(id, fileFullName, qqFile);
            };

            p.onComplete = function (i, fileFullName, res) {
                var qqFile = this;
                jv.StopLoading(true);

                if (res.msg) {
                    $(qqFile._element).trigger("removeOneFile");
                    alert(res.msg);
                    return;
                }

                res.qqFileId = i;

                if (p.listFile == "grid") {
                    TableFile.AppendItem(listFile, p, res);
                }
                else if (p.listFile == "list") {
                    ListFile.AppendItem(listFile, p, res);
                }

                if (p.on_Complete) p.on_Complete(i, fileFullName, res);
            };

            var instance = new qq.FileUploader(p);
            p.upFile = instance;
            self.data("upFile", instance);
            if (p.listFile == "grid") {
                var listFile = $(document.createElement("table")).insertAfter(this);
                new TableFile(listFile, p);
            }
            else if (p.listFile == "list") {
                var listFile = $(document.createElement("div")).insertAfter(this);
                new ListFile(listFile, p);
            }
        });

        $(".qq-upload-list", this).hide();
    };


    //强制触发。触发Hidden的change，input[readonly] 的change 等。不断完善。
    $.fn.EnforceTrigger = $.fn.enforceTrigger = function (eventName) {
        var check = function (jcon) {
            var con = jcon[0], tagName = con.tagName.toLowerCase();
            if (eventName == "change") {
                if (tagName == "input") {
                    if (jcon.attr("type") == "hidden") {
                        return true;
                    }
                    else if (jcon.attr("readonly")) {
                        return true;
                    }
                }
            }
            return false;
        };

        this.each(function () {
            var jd = $(this);
            if (check(jd)) jd.trigger(eventName);
        });

        return this;
    };

    $.fn.ReSet = $.fn.reSet = function () {
        var list = this.find("input");
        if (this.is("input")) { list.add(this);}
        for (var i = 0, len = list.length ; i < len; i++) {
            var self = list[i];
            var type = self.type;
            if (type == "button") continue;
            if (type == "hidden") continue;
            self.value = "";

            if (type == "file") {
                self.select();
                document.execCommand("Delete");
            }
        }

        var list = this.find("select");
        if (this.is("select")) { list.add(this); }
        for (var i = 0, len = list.length ; i < len; i++) {
            list[i].selectedIndex = 0;
        }
    };

    $.fn.MyCardTitle = $.fn.myCardTitle = function (setting) {
        var _shade = function (setting) {
            var height = 8,
            cv = setting.canvas,
            jod = $(cv);
            //            jod.parent().height(height);
            jod.attr("width", setting.width).attr("height", height);


            if ($.browser.msie && parseInt($.browser.version) < 9) {
                if (window.G_vmlCanvasManager) {
                    cv = window.G_vmlCanvasManager.initElement(cv);
                } else return false;
            }

            var ctx = cv.getContext('2d'),
            lg = ctx.createLinearGradient(0, 0, setting.width, 0);
            lg.addColorStop(0, setting.startColor);
            lg.addColorStop(1, setting.endColor);
            ctx.fillStyle = lg;
            ctx.fillRect(0, 0, setting.width, height);
            return true;
        };

        this.each(function (oi, od) {
            if ($(od).data("MyCardTitle") == true) return;
            $(od).data("MyCardTitle", true);
            if ($.browser.msie && (parseInt($.browser.version) <= 7)) {
                $(od).css("display", "inline");
            } else {
                $(od).css("display", "inline-block");
            }
            var pod = $(od).wrap("<div class='MyCardTitleWrap'></div>").parent().css("position", "relative"),
            cv = document.createElement("canvas");
            $(cv).css("position", "absolute").appendTo(pod);
            var container = pod.parent(),
            width = container.width() - $(od).width() - 3;
            if (width < 0) return;
            var endOpacity = parseInt($(od).attr("endopacity") || -50);
            if (endOpacity < 0) { width = 100 * width / (100 - endOpacity); }
            $(cv).css("bottom", 0).css("left", $(od).width() + parseInt($(od).css("paddingLeft")));
            _shade({ canvas: cv, startColor: $(od).css("backgroundColor"), endColor: jv.GetParentColor(pod), width: width, endOpacity: endOpacity });
        });
        return this;
    };


    $.fn.SmartPosition = $.fn.smartPosition = function ($target, width) {
        //弹出的 TextHelper position 须为  absolute
        //以 左上角为起点，顺时针计算依次为: a,b,c,d 四点。
        var jod = this;
        var thDiv = $target;
        var a = jod.offset(),
        c = { left: a.left + jod.outerWidth(), top: a.top + jod.outerHeight() },
        b = { left: c.left, top: a.top },
        d = { left: a.left, top: c.top };


        var $body = $(document.body);
        var body_width = $body.width();
        var body_height = $body.height();


        //对于TextHelper 来说， 内外宽度之差是固定的。
        //                        var th_diff_width= 
        //                            jv.GetInt( thDiv.css("marginLeft") ) +
        //                            jv.GetInt( thDiv.css("borderLeftWidth")) +
        //                            jv.GetInt( thDiv.css("paddingLeft")) +
        //                            jv.GetInt( thDiv.css("paddingRight")) + 
        //                            jv.GetInt( thDiv.css("borderRightWidth")) +
        //                            jv.GetInt ( thDiv.css("marginRight"))  ;
        var th_diff_width = 10;
        var th_diff_height = 10;

        //offset 会不断下移。

        thDiv.css("left", -3000).css("top", -3000).show();

        //智能算法， Input所在区间与offset 对应关系： 
        //如果在第一区间，肯定是d点
        //如果在第二区间，按d点设置宽度，获取高度，依次计算是否满足d,c
        //哪果在第三区间，按d点设置宽度，获取高度，依次计算是否满足 d,c,b
        //哪果在第四区间，按d点设置宽度，获取高度，依次计算是否满足 d,a

        if (a.left <= body_width / 2) {
            //第一区间
            if (a.top <= body_height / 2) {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);
                thDiv.offset(d);

                thDiv.css("maxHeight", body_height - d.top);
            }
                //第四区间
            else {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                var th_outerHeight = thDiv.outerHeight();
                if (d.top + th_outerHeight < body_height) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    //a点
                    var top = a.top - th_outerHeight;
                    thDiv.offset({ left: a.left, top: top > 0 ? top : 0 });

                    thDiv.css("maxHeight", a.top);
                }
            }
        }
        else {
            //第二区间：
            if (a.top <= body_height / 2) {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                //修改逻辑。 当在第二区间，按d点不出现折行时， 锁定d点。否则按c点。
                //if (d.top + thDiv.outerHeight() < body_height) {
                if (thDiv.outerWidth() < outerMaxWidth - th_diff_width) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    outerMaxWidth = width || d.left;
                    thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                    //c点
                    var th_outerHeight = thDiv.outerWidth();
                    var left = c.left - th_outerHeight;
                    thDiv.offset({ left: left > 0 ? left : 0, top: c.top });

                    thDiv.css("maxHeight", body_height - d.top);
                }
            }
                //第三区间
            else {
                var outerMaxWidth = width || (body_width - a.left);
                thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                //修改逻辑。 当在第三区间，按d点不出现折行时， 锁定d点。否则。。。
                //if (d.top + thDiv.outerHeight() < body_height) {
                if (thDiv.outerWidth() < outerMaxWidth - th_diff_width) {
                    thDiv.offset(d);

                    thDiv.css("maxHeight", body_height - d.top);
                } else {
                    //判断是否满足 c
                    outerMaxWidth = width || c.left;
                    thDiv.css("maxWidth", outerMaxWidth - th_diff_width);

                    var th_outerHeight = thDiv.outerHeight();

                    //修改逻辑。 当在第三区间，按c点不出现折行时， 锁定c点。否则。。。
                    //if (d.top + th_outerHeight < body_height) {
                    var th_outerWidth = thDiv.outerWidth();
                    if (th_outerWidth < outerMaxWidth - th_diff_width) {
                        //c点
                        var left = c.left - th_outerWidth;
                        thDiv.offset({ left: left > 0 ? left : 0, top: c.top });

                        thDiv.css("maxHeight", body_height - d.top);
                    } else {
                        //b点。
                        var left = b.left - th_outerWidth,
                        top = b.top - th_outerHeight;
                        thDiv.offset({
                            left: left > 0 ? left : 0,
                            top: top > 0 ? top : 0
                        });

                        thDiv.css("maxHeight", b.top);
                    }
                }
            }
        }
    };



    $.fn.PopTextArea = $.fn.popTextArea = function (jTextAreas) {
        this.focus(function () {
            var self = $(this);
            var size = jv.getEyeSize();
            var ta = '<textarea style="width:100%;padding:0px;margin:0px;height:' + (size.height - 240) + 'px">' + this.value + "</textarea>";
            var boxy = Boxy.html(ta, ["确 定", "取 消"], function (v, bxy) {
                if (v == "确 定") {
                    self.val(bxy.boxy.find("textarea").val());
                }
            }, { width: size.width - 200, title: "输入文本 - " + (self.closest(".kv").find(".key").text() || "") });

            boxy.boxy.find("textarea").focus();
            boxy.boxy.find(".answers .button[value='确 定']").addClass("large");
        });
    };

    $.fn.BindResize = $.fn.bindResize = function (events) {
        var self = this[0];
        //移动事件
        var mouseMove = function (e) {
            if (events.move) {
                events.move(self, e);
            }
        };

        //停止事件
        var mouseUp = function (e) {
            //在支持 releaseCapture 做些东东
            if (self.releaseCapture) {
                //释放焦点
                self.releaseCapture();
                //移除事件
                self.onmousemove = self.onmouseup = null;
            }
            else {
                //卸载事件
                $(document).unbind("mousemove", mouseMove).unbind("mouseup", mouseUp);
            }

            if (events.up) {
                events.up(self, e);
            }
        };


        this.mousedown(function (e) {
            //在支持 setCapture 做些东东
            if (self.setCapture) {
                //捕捉焦点
                self.setCapture();
                //设置事件
                self.onmousemove = function (ev) {
                    mouseMove(ev || event)
                };
                self.onmouseup = mouseUp;
            } else {
                //绑定事件
                $(document).bind("mousemove", mouseMove).bind("mouseup", mouseUp);
            }
            //防止默认事件发生
            e.preventDefault();

            if (events.down) {
                events.down(self, e);
            }
        });
    }

})(jQuery);



(function () {

    //本页标志性含数 getDoer .
    if (jv.FindPopConfig) return;

    jv.FindPopConfig = jv.findPopConfig = function (Dict, area, controller, action) {
        action = action || "";

        //额外的可以配置指定的List
        //Dict.cs["TContract/List"] = [600,0] ;

        //如果area 不存在,在根路径下找. 
        //参数方式: cs,Common,List
        //cs,Common
        //Common

        if (!controller && !action && area) {
            controller = area;
            area = "";
        }
        Dict[area] = Dict[area] || {};
        if (area) {
            return Dict[area][controller + "/" + action] || Dict[area][controller] || Dict[controller] || [];
        }
        else {
            for (var p in Dict) {
                return Dict[controller + "/" + action] || Dict[controller] || []
            }
        }

        return [];
    }
    jv.PopListConfig = jv.popListConfig = function (area, controller, action) {
        // 正数代表 宽度, 高度, 0 代表自适应.  负数代表 左右边距, 上下边距
        //第三个参数，表示打开方式： true：window.open, false：   window.showModelDialog (默认值) , 《Boxy：过滤器》： 使用Boxy 打开  , 字符串：open的name。
        var Dict = { App: {}, Admin: {}, Report: {}, Master: {}, Cost: {}, Property: {}, Sys: {} };
        Dict.Admin["Role"] = [350, -50];

        Dict.App["TRegion"] = [500, 0];

        Dict.Master["Pop/CostItem"] = [500, -50];
        Dict.Master["Pop/BuildRoomList"] = [800, -50];

        Dict["Notices"] = [-1, -1, true];
        Dict["BathRoom"] = [700, 425];

        return jv.FindPopConfig(Dict, area, controller, action);
    };

    jv.PopDetailConfig = jv.popDetailConfig = function (area, controller, action) {
        // 正数代表 宽度, 高度, 0 代表自适应.  负数代表 左右边距, 上下边距
        //第三个参数，表示打开方式： true：window.open, false：   window.showModelDialog (默认值) , 《Boxy：过滤器》： 使用Boxy 打开  , 字符串：open的name。
        var Dict = { App: {}, Admin: {}, Report: {}, cs: {} };
        Dict.Admin["Role"] = [550, 250, ".Main"];
        Dict.Admin["Menu"] = [710, 450, ""];

        Dict.App["TRegion"] = [650, 250, ".Main"];
        Dict["PopSearch"] = [450, 450];
        Dict["QuerySet"] = [605, 433];
        Dict["PopFeeDelReason"] = [450, 350];

        Dict["Recp"] = [900, 700, ""];
        Dict["Notices"] = [-1, -1, true];


        Dict["Power"] = [-50, -50, true];
        return jv.FindPopConfig(Dict, area, controller, action);
    };


})();
(function () {

    //本页标志性含数 getDoer .
    if (jv.popPrint) return;

    jv.PopPrint = jv.popPrint = function (urlSetting) {
        urlSetting = $.extend({ url: jv.Root + "ReportWeb/" + jv.page().controller + "/" + jv.page().action + ".aspx", query: false }, urlSetting);
        if (urlSetting.query) {
            urlSetting.url += "?" + urlSetting.query;
        }

        jv.PopDetail({ url: urlSetting.url, data: { code: true } });
        return false;
    };

    jv.PopSearch = jv.popSearch = function (querySelector) {
        if (querySelector) {
            window.PopSearch = function () { $(querySelector).trigger("click"); };
        }

        jv.PopDetail({ url: jv.Root + "html/PopSearch.html", data: { code: true }, entity: "PopSearch" });
        return false;
    };


})();
/*
重新定义：
1.利用 CSS3 标准，对元素增加属性 border-radius ，指定左上、左下、右上、右下。  如 border-radius=5,0,0,5 。数值表示半径。
该Div对象需定义为：
1. position= absolute or relative
2. 去除 overflow 属性。
3. display = block or inline-block
4. 尽量不要有 line-height ， 在IE7下会显示上下错位。
*/

(function ($) {
    jv.CssHasBorderRadius = (typeof (document.createElement("div").style.borderRadius) == "string");
    /*
    指定圆角半径大小，单数值，或多数值。
    参数：{radius：,func:,unCorner:,backColor:}
    */
    $.fn.MyCorner = function (setting) {
        var setting = $.extend({}, setting);
        var _GetCorner = function (jod, radius) {
            if (!radius) {
                radius = jod.attr("border-radius");
            }
            if (!radius) {
                radius = setting.radius;
            }

            var _GetNotAutoSetting = function (radius) {
                var Corner = Array();
                if (radius instanceof Array) return radius;
                if ((radius + "").indexOf(",") > 0) {
                    Corner = radius.split(',');
                    for (var i = 0; i < 4; i++) {
                        Corner[i] = parseInt(Corner[i]);
                    }
                }
                else if ((radius + "").indexOf(" ") > 0) {
                    Corner = radius.split(' ');
                    for (var i = 0; i < 4; i++) {
                        Corner[i] = parseInt(Corner[i]);
                    }
                }
                else {
                    for (var i = 0; i < 4; i++) {
                        Corner.push(parseInt(radius));
                    }
                }
                return Corner;
            },
            _GetAutoSetting = function (jobj) {
                if (jobj.filter("body").length > 0) return [0, 0, 0, 0];
                if (jobj.filter("[border-radius]").length > 0) {
                    if (jobj.attr("border-radius") == "auto") {
                        var pr = _GetAutoSetting(jobj.parent());
                        for (var i = 0; i < 4; i++) {
                            pr[i] -= parseInt(jobj.parent().css("borderTopWidth"));
                            if (pr[i] < 0) pr[i] = 0;
                        }

                        jobj.attr("border-radius", pr);
                        return pr;
                    }
                    else return _GetNotAutoSetting(jobj.attr("border-radius"));
                }
                else return [0, 0, 0, 0];
            };
            if (radius && radius != "auto") return _GetNotAutoSetting(radius);
            else return _GetAutoSetting(jod);
        };

        var THIS = this;
        var setCssRadius = function () {
            $(THIS).each(function () {
                var self = $(this);
                if (jv.GetInt(self.css("borderRadius")) == 0) {
                    var radiusAry = _GetCorner(self);
                    $(radiusAry).each(function (_i, _d) {
                        radiusAry[_i] = _d + "px";
                    });

                    self.css("borderRadius", radiusAry.join(" "));
                }
            });
            return THIS;
        };
        if (jv.CssHasBorderRadius) {
            return setCssRadius();
        }
        else if (!window.G_vmlCanvasManager) {
            return setCssRadius();
        }
        else if ($.browser.mozilla) {
            if (parseInt($.browser.version) > 2) {
                return setCssRadius();
            }
        }
        else if ($.browser.safari) {
            return setCssRadius();
        }


        var _unCorner = function (jod, unCorner) {
            if (unCorner != true) return false;
            if (!jod.data("myCorner")) return true;
            $(jod.data("myCorner")).each(function () {
                $(this).remove();
            });
            jod.data("myCorner", null);
        },
        _corner = function (jod, setting) {
            var p = $.extend({}, setting, p),
            radius = p.radius, func = p.func, unCorner = p.unCorner;

            if (_unCorner(jod, unCorner) == true) return;

            var Corner = _GetCorner(jod, radius),

            isValid = false;
            $(Corner).each(function (oi, od) {
                if (od > 0) { isValid = true; return false; }
            });
            if (isValid == false) {
                return false;
            }
            if (jod.data("myCorner")) return true;

            if (["relative", "absolute", "fixed"].indexOf(jod.css("position")) < 0) {
                jod.css("position", "relative"); // absolute 也可以。
            }

            if (["block", "inline-block", "none"].indexOf(jod.css("display")) < 0) {
                jod.css("display", "block"); // absolute 也可以。
            }

            if (jod.css("overflow")) {
                jod.css("overflow", "");
            }
            var borderTopWidth = parseInt(jod.css("borderTopWidth")) || 0,
            borderBottomWidth = parseInt(jod.css("borderBottomWidth")) || 0,
            borderLeftWidth = parseInt(jod.css("borderLeftWidth")) || 0,
            borderRightWidth = parseInt(jod.css("borderRightWidth")) || 0,

            v0 = document.createElement("canvas"),
            v1 = document.createElement("canvas"),
            v2 = document.createElement("canvas"),
            v3 = document.createElement("canvas");

            $(v0).css("top", "0px").css("left", "0px").css("marginLeft", 0 - borderLeftWidth).css("marginTop", 0 - borderTopWidth);
            $(v1).css("top", "0px").css("right", "0px").css("marginRight", 0 - borderRightWidth).css("marginTop", 0 - borderTopWidth);
            $(v2).css("bottom", "0px").css("right", "0px").css("marginRight", 0 - borderRightWidth).css("marginBottom", 0 - borderBottomWidth);
            $(v3).css("bottom", "0px").css("left", "0px").css("marginLeft", 0 - borderLeftWidth).css("marginBottom", 0 - borderBottomWidth);


            var jvs = $([v0, v1, v2, v3]);
            //            var outerWidth = parseInt(jod.css("outlineWidth"));
            //var div = $('<div style="display: inherit;" />').appendTo(jod);
            jvs.each(function (oi, od) {
                if (Corner[oi] <= 0) return;

                if (window.G_vmlCanvasManager) {
                    od = window.G_vmlCanvasManager.initElement(od);
                }

                var jvod = $(od),
                borderWidth = (oi % 2 == 0) ? borderTopWidth : borderBottomWidth;

                if (borderWidth == 0) return;
                jvod.css("position", "absolute");
                //                jvod.css("margin", 0 - borderWidth);


                //                var size = borderWidth > Corner[oi] ? Corner[oi] / 2 + borderWidth : Corner[oi] + borderWidth / 2;
                //                jvod.attr("width", size);
                //                jvod.attr("height", size);
                jvod.appendTo(jod);
                var ary = (jod.data("myCorner") || []);
                ary.push(od);
                jod.data("myCorner", ary);
            });

            jod.data("backColor", p.backColor);
            if (Corner[0] > 0 && borderTopWidth > 0 && borderLeftWidth > 0) {
                var v = v0,
                 c = Corner[0],
                 b = Math.min(borderTopWidth, borderLeftWidth),
                 borderColor = borderTopWidth >= borderLeftWidth ? jod.css("borderTopColor") : jod.css("borderLeftColor");
                $(v).attr("width", c + b / 2).attr("height", c + b / 2);

                _Arc({
                    canvas: v,
                    x: c,
                    y: c,
                    radius: c,
                    start: Math.PI * 1.5,
                    p1: { x: 0 - c, y: c },
                    p2: { x: 0 - c, y: 0 - c },
                    p3: { x: c, y: 0 - c },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[1] > 0 && borderTopWidth > 0 && borderRightWidth > 0) {
                var v = v1,
                 c = Corner[1],
                 b = Math.min(borderTopWidth, borderRightWidth),
                 borderColor = borderTopWidth >= borderRightWidth ? jod.css("borderTopColor") : jod.css("borderRightColor");

                $(v).attr("width", c).attr("height", c);

                _Arc({
                    canvas: v,
                    x: 0,
                    y: c,
                    radius: c,
                    start: Math.PI * 2,
                    p1: { x: 0, y: 0 - c },
                    p2: { x: c * 2, y: 0 - c },
                    p3: { x: c * 2, y: c },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[2] > 0 && borderBottomWidth > 0 && borderRightWidth > 0) {
                var v = v2,
                c = Corner[2],
                b = Math.min(borderBottomWidth, borderRightWidth),
                borderColor = borderBottomWidth >= borderRightWidth ? jod.css("borderBottomColor") : jod.css("borderRightColor");

                $(v).attr("width", c).attr("height", c);


                _Arc({
                    canvas: v,
                    x: 0,
                    y: 0,
                    radius: c,
                    start: Math.PI * 0.5,
                    p1: { x: c * 2, y: 0 },
                    p2: { x: c * 2, y: c * 2 },
                    p3: { x: 0, y: c * 2 },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if (Corner[3] > 0 && borderBottomWidth > 0 && borderLeftWidth > 0) {
                var v = v3,
                 c = Corner[3],
                 b = Math.min(borderBottomWidth, borderLeftWidth),
                 borderColor = borderBottomWidth >= borderLeftWidth ? jod.css("borderBottomColor") : jod.css("borderLeftColor");

                $(v).attr("width", c).attr("height", c);

                _Arc({
                    canvas: v,
                    x: c,
                    y: 0,
                    radius: c,
                    start: Math.PI,
                    p1: { x: c, y: 2 * c },
                    p2: { x: 0, y: 2 * c },
                    p3: { x: 0 - c, y: 0 },
                    pjdiv: jod,
                    func: func,
                    borderColor: borderColor,
                    borderWidth: b
                });
            }
            if ($.browser.msie && $.browser.version < 9) {
                jvs.each(function () {
                    var self = $(this);
                    self.find("div").width(self.width()).height(self.height());
                });
            }
            return true;
        },



        /* 利用 */
        _Arc = function (setting) { //outColor, borderColor, borderWidth) {
            var canvas = setting.canvas,
                x = setting.x,  //X坐标
                y = setting.y,  //Y坐标
                radius = setting.radius,    //半径
                start = setting.start,      // 起始角度
                p1 = setting.p1,
                p2 = setting.p2,
                p3 = setting.p3,
                pjdiv = setting.pjdiv,
                borderWidth = setting.borderWidth,
                borderColor = setting.borderColor,

                outColor = pjdiv.data("backColor") || jv.GetParentColor(pjdiv);
            borderColor = borderColor || "white";
            //            var borderWidth = parseInt(pjdiv.css("borderTopWidth"));

            var bigBgFunc = function (v) {
                if (v > 0) v += borderWidth * 2;
                else v -= borderWidth * 2;
                return v;
            };
            p1.x = bigBgFunc(p1.x);
            p1.y = bigBgFunc(p1.y);
            p2.x = bigBgFunc(p2.x);
            p2.y = bigBgFunc(p2.y);
            p3.x = bigBgFunc(p3.x);
            p3.y = bigBgFunc(p3.y);
            //setting.p1 = p1; setting.p2 = p2; setting.p3 = p3;

            var ctx = canvas.getContext('2d');

            if (radius.func) {
                if (radius.func(ctx, radius) == false) return;
            }
            ctx.beginPath();
            if (borderWidth > 0) {
                ctx.lineWidth = borderWidth * 2;
                ctx.strokeStyle = borderColor;
                ctx.arc(x, y, radius, start, start - Math.PI / 2.0, true);  //逆时针
                ctx.stroke();
            }

            ctx.lineWidth = 0;
            ctx.lineTo(p1.x, p1.y);
            ctx.lineTo(p2.x, p2.y);
            ctx.lineTo(p3.x, p3.y);
            ctx.fillStyle = parseInt(pjdiv.css("outlineWidth")) > 0 ? borderColor : outColor;
            ctx.fill();
        }

        this.each(function () {
            _corner($(this), setting);
        });
        return this;
    };

    $.fn.MyShade = function (setting) {

    }
    $.fn.MyCardTitle = function (setting) {
        var _shade = function (setting) {
            var height = 8,
            cv = setting.canvas,
            jod = $(cv);
            //            jod.parent().height(height);
            jod.attr("width", setting.width).attr("height", height);


            if ($.browser.msie && parseInt($.browser.version) < 9) {
                if (window.G_vmlCanvasManager) {
                    cv = window.G_vmlCanvasManager.initElement(cv);
                }
            }

            var ctx = cv.getContext('2d'),
            lg = ctx.createLinearGradient(0, 0, setting.width, 0);
            lg.addColorStop(0, setting.startColor);
            lg.addColorStop(1, setting.endColor);
            ctx.fillStyle = lg;
            ctx.fillRect(0, 0, setting.width, height);
            return true;
        };

        this.each(function () {
            var self = $(this);
            if (self.data("MyCardTitle") == true) return;
            self.data("MyCardTitle", true);
            if ($.browser.msie && (parseInt($.browser.version) <= 7)) {
                self.css("display", "inline");
            } else {
                self.css("display", "inline-block");
            }
            var pod = self.wrap("<div class='MyCardTitleWrap'></div>").parent().css("position", "relative"),
            cv = document.createElement("canvas");
            $(cv).css("position", "absolute").appendTo(pod);
            var container = pod.parent(),
            width = container.width() - self.width() - 3;
            if (width < 0) return;
            var endOpacity = parseInt(self.attr("endopacity") || -50);
            if (endOpacity < 0) { width = 100 * width / (100 - endOpacity); }
            $(cv).css("bottom", 0); //.css("left", $(od).width() + parseInt($(od).css("paddingLeft")));
            _shade({ canvas: cv, startColor: self.css("backgroundColor"), endColor: jv.GetParentColor(pod), width: width, endOpacity: endOpacity });
        });
        return this;
    }
})(jQuery);
