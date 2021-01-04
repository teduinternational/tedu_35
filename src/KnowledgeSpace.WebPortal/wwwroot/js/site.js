// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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
        return 'khoảng ' + Math.round(elapsed / msPerDay) + ' ngày trước';
    }

    else if (elapsed < msPerYear) {
        return 'khoảng ' + Math.round(elapsed / msPerMonth) + ' tháng trước';
    }

    else {
        return 'khoảng ' + Math.round(elapsed / msPerYear) + ' năm trước';
    }
}