
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
