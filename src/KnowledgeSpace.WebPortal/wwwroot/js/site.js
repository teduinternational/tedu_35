// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function formatDate(date) {
    if (date === undefined)
        date = new Date();
    if ((date instanceof Date) === false)
        date = new Date(date);

    var day = new Date(date).getDate();
    var month = date.getMonth();
    month += 1;  // JavaScript months are 0-11
    var year = date.getFullYear();

    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();

    return day + '/' + month + '/' + year + ' ' + hour + ':' + minute + ':' + second;
}

function formatRelativeTime(fromDate) {
    if (fromDate === undefined)
        fromDate = new Date();
    if ((fromDate instanceof Date) === false)
        fromDate = new Date(fromDate);
    var msPerMinute = 60 * 1000;
    var msPerHour = msPerMinute * 60;
    var msPerDay = msPerHour * 24;
    var msPerMonth = msPerDay * 30;
    var msPerYear = msPerDay * 365;

    var elapsed = new Date() - fromDate;

    if (elapsed < msPerMinute) {
        return Math.round(elapsed / 1000) + ' giây trước';
    }

    else if (elapsed < msPerHour) {
        return Math.round(elapsed / msPerMinute) + ' phút trước';
    }

    else if (elapsed < msPerDay) {
        return Math.round(elapsed / msPerHour) + ' giờ trước';
    }

    else if (elapsed < msPerMonth) {
        return 'approximately ' + Math.round(elapsed / msPerDay) + ' ngày trước';
    }

    else if (elapsed < msPerYear) {
        return 'approximately ' + Math.round(elapsed / msPerMonth) + ' tháng trước';
    }

    else {
        return 'approximately ' + Math.round(elapsed / msPerYear) + ' năm trước';
    }
}