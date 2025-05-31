
var TodayMade = new Date();
var dateToday = TodayMade.getDate();
var monthToday = TodayMade.getMonth() + 1; //January is  0;
var yearToday = TodayMade.getFullYear();
if (dateToday < 10) {
    dateToday = '0' + dateToday
}
if (monthToday < 10) {
    monthToday = '0' + monthToday;
}
TodayMade = yearToday + '-' + monthToday + '-' + dateToday;

function GetTrangThaiString(trangThai) {
    if (trangThai == 0) {
        return "Chưa duyệt";
    }
    else if (trangThai == 1) {
        return "Chưa trả";
    }
    else if (trangThai == 2) {
        return "Đã duyệt"
    }
    else if (trangThai == 3) {
        return "Đã trả"
    }
    else if (trangThai == 4) {
        return "Từ chối"
    }
    else if (trangThai == 5) {
        return "Hủy phiếu"
    }
    else if (trangThai == 6) {
        return "Không mượn"
    }
    return ""; // Trạng thái không xác định
}




