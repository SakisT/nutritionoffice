$(document).ready(function () {
    $('.ExportToButton').on('click', function () {
        var parent = $(this).parents('.ExportTo').find('select option:selected');
        var link = $(this).data('link');
        link = link.replace('filetypetext', parent.val().toUpperCase());
        window.location = link;
    });

    $('.selectall').on('mouseup', function (event) {
        event.preventDefault();
        $(this).select();
    });
    $('#appointmentdate').on('change', function () {
        var newdatestring = $(this).val();
        $('#appointmentmessage').val('Σας υπενθυμίζουμε το ραντεβού μας για ' + newdatestring + ' στις ' + $('#fromtime').val());
        var parts = newdatestring.split('/');
        var newrealdate = new Date(parseInt(parts[2]), parseInt(parts[1]) - 1, parseInt(parts[0]));
        var timepart = $("#remindon").val().split(' ')[1];
        if (timepart === null) { timepart = "11:00"; }
        var end_date = new Date(parseInt(parts[2]), parseInt(parts[1]) - 1, parseInt(parts[0]));
        end_date.setDate(newrealdate.getDate() - 2);
        $("#remindon").val(end_date.getDate() + '/' + (end_date.getMonth() + 1) + '/' + end_date.getFullYear() + ' ' + timepart);
    });
    $('#appointmentcustomer').on('change', function (event) {
        var customerid = $(this).val();
        if (customerid > 0) {
            var serviceURL = $('#variantdata').data('getcustomerdatalink');
            $.getJSON(serviceURL, { id: customerid }, function (data) {
                $('#email').val(data.email);
                $('#mobile').val(data.Mobile);
            });
        }
        else {
            $('#email').val('');
            $('#mobile').val('');
        }
    });
    $('#fromtime').on('change', function () {
        var newdatestring = $('#appointmentdate').val();
        var newtime = $(this).val();
        $('#totime').val(newtime);
        $('#appointmentmessage').val('Σας υπενθυμίζουμε το ραντεβού μας για ' + newdatestring + ' στις ' + $(this).val());
    });
    $('#sendtoallmobiles').change(function () {
        var ischecked = $(this).prop('checked');
        $('.selectmobile').each(function () {
            $(this).prop('checked', ischecked);
        });
    });
    $('#displaymeasurementstats').on('click', function (event) {
        event.preventDefault();
        var measurementsid = new Array();

        $('.CustomerMeasurementRow:checked').each(function (index) {
            measurementsid.push($(this).data('itemid'));
        });
        var link = $(this).attr('href');
        var data = measurementsid.join(',');
        window.location = link + '?measurementids=' + data;
    });
    $('#sendtoallemails').change(function () {
        var ischecked = $(this).prop('checked');
        $('.selectallemails').each(function () {
            $(this).prop('checked', ischecked);
        });
    });
    $('#checkall').change(function () {
        var ischecked = $(this).prop('checked');
        $('.checkall').each(function () {
            $(this).prop('checked', ischecked);
        });
    });
    $('#sendsmstoall').change(function () {
        var ischecked = $(this).prop('checked');
        $('.sendsmstoall').each(function () {
            $(this).prop('checked', ischecked);
        });
    });
    $('#sendemailtoall').change(function () {
        var ischecked = $(this).prop('checked');
        $('.sendemailtoall').each(function () {
            $(this).prop('checked', ischecked);
        });
    });
    $('.savefood').click(function () {
        var foodid = $(this).closest('tr').attr('id');
        var newenglishname = $(this).closest('tr').find('[class*="foodenglishname"]').first().val();
        var newgreekname = $(this).closest('tr').find('[class*="foodgreekname"]').first().val();
        var newisbreakfast = $(this).closest('tr').find('[class*="foodisbreakfast"]').first().is(':checked');
        var newissnack = $(this).closest('tr').find('[class*="foodissnack"]').first().is(':checked');
        var newislunch = $(this).closest('tr').find('[class*="foodislunch"]').first().is(':checked');
        var newisdinner = $(this).closest('tr').find('[class*="foodisdinner"]').first().is(':checked');
        var newiscollagene = $(this).closest('tr').find('[class*="foodiscollagene"]').first().is(':checked');
        var newisantioxidant = $(this).closest('tr').find('[class*="foodisantioxidant"]').first().is(':checked');
        var newisdetox = $(this).closest('tr').find('[class*="foodisdetox"]').first().is(':checked');
        var newisdiatrofogenomiki = $(this).closest('tr').find('[class*="foodisdiatrofogenomiki"]').first().is(':checked');
        var newismenopause = $(this).closest('tr').find('[class*="foodismenopause"]').first().is(':checked');
        var url = $(this).closest('table').attr('saveurl');
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            data: { id: foodid, englishname: newenglishname, greekname: newgreekname, isbreakfast: newisbreakfast, issnack: newissnack, islunch: newislunch, isdinner: newisdinner, iscollagene: newiscollagene, isantioxidant: newisantioxidant, isdetox: newisdetox, isdiatrofogenomiki: newisdiatrofogenomiki, ismenopause: newismenopause },
            success: function (data) {
                alert(data.response);
            },
            async: true
        });

    });
    $('.deletefood').click(function () {
        var foodid = $(this).closest('tr').attr('id');
        var newenglishname = $(this).closest('tr').find('[class*="foodenglishname"]').first().val();
        var newgreekname = $(this).closest('tr').find('[class*="foodgreekname"]').first().val();
        var url = $(this).closest('table').attr('deleteurl');
        var message = "Are you sure that you want delete food :\r\n" + newenglishname + "\r\n" + newgreekname + "?";
        if (confirm(message)) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: url,
                data: { id: foodid },
                success: function (data) {
                    if (data.response === "Delete was successful") {
                        location.reload();
                    }
                    else {
                        alert(data.response);
                    }
                },
                async: true
            });
        }
    });
    $('.saveuser').click(function () {
        var currentuserid = $(this).closest('tr').attr('userid');
        var newrole = $(this).closest('tr').find('.userrole').val();
        var newcompany = $(this).closest('tr').find('.usercompany').val();
        var newpassword = $(this).closest('tr').find('.userpassword').val();
        var url = $('#saveurl').attr('url');

        $.ajax({
            type: "POST",
            dataType: "json",
            url: url,
            data: { userid: currentuserid, userrole: newrole, usercompany: newcompany, userpassword: newpassword },
            success: function (data) {
                alert(data.response);
            },
            async: false
        });

    });
    $('.displaymeasurementdetails').on('click', function () {
        var link = $(this).data('link');
        var title = $(this).data('title');
        $("#measurementdetailview").load(link);
        setTimeout(function () {
            CreateBMIChart();
        }, 500);
        $("#measurementdetailview").dialog("option", "title", title);
        $("#measurementdetailview").dialog("open");
    });
    $(function () {
        var $HomeIndex = $('currentdatecustmersdata');
        var $company = $('#layoutcompanyid');
        var count, tip;
        if ($company.length !== 0) {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: $company.data('link'),
                success: function (data) {
                    if (data.NameDayCustomers.length > 0) {
                        tip = data.NameDayTooltip;
                        count = data.NameDayCustomers.length;
                        $company.append('<small><span style="background:#ff6a00" onclick="NavigateToNameDays()" class="badge" title="' + tip + '">' + count + '</span></small>');

                        //Αν είμαστε στη Home Index
                        //if ($HomeIndex.length !== 0) {

                        //}
                    }
                    if (data.BirthDayCustomers.length > 0) {
                        tip = data.BirthDayTooltip;
                        count = data.BirthDayCustomers.length;
                        $company.append('<small><span style="background:#f374f5" onclick="NavigateToNameDays()" class="badge" title="' + tip + '">' + count + '</span></small>');

                        //Αν είμαστε στη Home Index
                        //if ($HomeIndex.length !== 0) {

                        //}
                    }
                },
                async: false
            });
        }
    });
    $('.copymeal').on('click', function (event) {
        event.preventDefault();
        var link = $(this).attr('href');
        var dayindex = $(this).attr('dayindex');
        var mealindex = $(this).attr('mealindex');
        var mealid = $(this).attr('mealid');
        //alert('dayindex=' + dayindex + 'mealindex=' + mealindex + 'mealid=' + mealid);
        var updatelink = $('#fooddialog').attr('updatesingledatemeallink')
        var updatedaytotalslink = $('#fooddialog').attr('updatedaytotalslink') + '&dayindex=' + dayindex;
        var $noteitem = $('#note-' + dayindex + '-' + mealindex);
        $.ajax({
            type: "POST",
            dataType: "json",
            url: link,
            success: function (data) {
                if (data.Result === "Success") {
                    //Αν έχουμε ζητήσει Copy σε όλες τις ημέρες
                    //Κάνουμε Refresh όλη τη σελίδα
                    if (data.UpdateDay === -1) {
                        window.location.reload();
                    }
                    else {

                        //Κάνουμε Refresh μόνο το meal στο οποίο
                        //αντιγράψαμε τα δεδομένα
                        $('#mealid-' + dayindex + '-' + mealindex).load(updatelink + '/' + mealid);
                        $('#day-' + dayindex).load(updatedaytotalslink);
                        $noteitem.val(data.Notes);
                    }
                }
                else {
                    alert(data.Result);
                }
            },
            async: true
        });
    });
    $('.recipecategorylink').on('click', function () {
        var link = $(this).data('link');
        $('#categoryrecipies').load(link);
    });
    $('#diettypeid').on('change', function () {
        var newvalue = $(this).val()
        $('#diettype').val(newvalue);
    });
    $('a#emaildiet').on('click', function (event) {
        event.preventDefault();
        var link = $('#attachmentsrender').data('link')
        $('#attachmentsrender').load(link);
        $('#sendbymaildialog').dialog('open');
    });
    $('#helplink').on('click', function () {
        var link = $('#helplink').data('link');
        $('#helpcontainerframe').attr('src', link);
        $('#helpcontainer').dialog('open');
    });
    $('#displaysendrecallLink').on('click', function () {
        var customermail = $('#sendcustomerrecalllink').data('customeremail');
        if (customermail !== null && customermail !== isNaN && customermail !== '') {
            $('#sendcustomerrecalllink').dialog("option", "title", customermail);
            $('#sendcustomerrecalllink').dialog("open");
        }
        else {
            alert("Ο πελάτης δεν έχει email.");
        }

    });
    $('#displaysendbasicquestLink').on('click', function () {
        var customermail = $('#sendcustomerbasiquestlink').data('customeremail');
        if (customermail !== null && customermail !== isNaN && customermail !== '') {
            $('#sendcustomerbasiquestlink').dialog("option", "title", customermail);
            $('#sendcustomerbasiquestlink').dialog("open");
        }
        else {
            alert("Ο πελάτης δεν έχει email.");
        }

    });
    $('#senddailyrecalllinktocustomer').on('click', function () {
        var customerid = $('#sendcustomerrecalllink').data('customerid');
        var mailmessagebodytext = $('#mailmessagerecalbodytext p').html();
        var link = $('#sendcustomerrecalllink').data('link');

        $.ajax({
            url: link,
            type: "POST",
            dataType: "json",
            data: { id: customerid, MessageText: mailmessagebodytext },
            success: function (data) {
                if (data.Result === "Success") {
                    alert('Απεστάλλει');
                    $('#sendcustomerrecalllink').dialog("close");
                }
                else {
                    alert(data.Result);
                }
            }
        })
    });
    $('#sendbasicquestlinktocustomer').on('click', function () {
        var customerid = $('#sendcustomerbasiquestlink').data('customerid');
        var mailmessagebodytext = $('#mailmessagebasicquestbodytext p').html();
        var link = $('#sendcustomerbasiquestlink').data('link');

        $.ajax({
            url: link,
            type: "POST",
            dataType: "json",
            data: { id: customerid, MessageText: mailmessagebodytext },
            success: function (data) {
                if (data.Result === "Success") {
                    alert('Απεστάλλει');
                    $('#sendcustomerrecalllink').dialog("close");
                }
                else {
                    alert(data.Result);
                }
            }
        })
    });
    $('#createnewcustomerbutton').on('click', function () {
        var checklink = $('#createnewcustomer').data('checklink');
        var lastname = $('#lastname').val();
        var firstname = $('#firstname').val();
        $.ajax({
            url: checklink,
            type: "GET",
            dataType: "json",
            data: { lastname: lastname, firstname: firstname },
            success: function (data) {
                if (data.Exists !== true) {
                    $('#createnewcustomer').submit();
                }
                else {
                    if (confirm('Υπάρχει ήδη ένας πελάτης με όνομα ' + data.customername +
                        '\r\n της ομάδας ' + data.TargetGroupName + '.' +
                        '\r\n πατήστε "OK" για να συνεχίσετε στην αποθήκευση,\r\n ή "Cancel" για να διορθώσετε τον υπάρχοντα πελάτη!')) {
                        $('#createnewcustomer').submit();
                    }
                    else {
                        var editlink = $('#createnewcustomer').data('editlink') + '/' + data.id;
                        window.location = editlink;
                    }
                }
            }
        })
    });
    $('.btn-upload').on('change', function () {
        var icon = $(this).parent().find('.glyphicon-upload');
        icon.css('color', 'red');
    });
    $('#getsmscredits').on('click', function () {
        var link = $(this).data('link');
        $.ajax({
            type: "GET",
            url: link,
            success: function (data) {
                if (data.Result === "Success") {
                    $('#getsmscredits').text(data.Credits);
                }

            }
        });
    });



});

$(document).on('keyup', '.numbertextbox', function () {
    var text = $(this).val();
    text = text.toString().replace(/,/g, '.');
    $(this).val(text);
});

$(document).on('keydown', '.numbertextbox', function (e) {
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});

$(document).on('mouseup', '.numbertextbox', function () {
    $(this).select();
});

$(function () {
    $(document).tooltip({
        content: function () {
            var element = $(this);
            return element.attr('title');
        }
    });
});

$(function () {
    var initialLocaleCode = 'el';
    var link = $('#calendar').data('indexlink');
    if (link != undefined) {
        $('#calendar').fullCalendar({
            customButtons: {
                neweventbutton: {
                    text: 'New',
                    click: function () {
                        var createlink = $('#calendar').data('createappointmentlink');
                        var view = $('#calendar').fullCalendar('getView');
                        if (view.name === "agendaDay") {
                            createlink = createlink + "?datetime=" + $('#calendar').fullCalendar('getDate').format();
                        }
                        window.location = createlink;
                    }
                }
            },
            header: {
                left: 'prev,next today',
                center: 'neweventbutton title',
                right: 'month,agendaWeek,agendaDay,listMonth'
            },
            businessHours: {
                start: '10:00', // a start time (10am in this example)
                end: '22:00', // an end time (6pm in this example)
            },
            scrollTime: '09:00:00',
            defaultView: 'agendaWeek',
            theme: true,
            views: {
                month: {
                    columnFormat: 'dddd'
                },
                agendaWeek: {
                    columnFormat: 'ddd D/M',
                },
                agendaDay: {
                    columnFormat: 'dddd D MMMM YYYY',
                }
            },
            defaultDate: $('#calendar').data('startdate'),
            //columnFormat: 'dddd',
            locale: initialLocaleCode,
            buttonIcons: false, // show the prev/next text
            weekNumbers: false,
            nowIndicator: true,
            timeFormat: 'H:mm',
            navLinks: true, // can click day/week names to navigate views
            editable: true,
            eventLimit: true, // allow "more" link when too many events
            slotDuration: '00:10:00',//Event timespan
            snapDuration: '00:10:00',
            slotLabelInterval: '00:15:00',
            displayEventEnd: false,
            //viewRender: function (view, element) {
            //    //RefreshAppointments();
            //},
            dayClick: function (date, jsevent, view) {
                //debugger;
            },
            //events:link,
            events: function (start, end, timezone, callback) {
                var noTime = $('#calendar').fullCalendar('getDate').format();
                $.ajax({
                    url: link,
                    data: { date: noTime },
                    success: function(doc) {
                        callback(doc);
                    }
                })},
            //eventMouseover: function (calEvent) {
            //    //debugger;
            //},
            eventClick: function (calEvent, jsEvent, view) {
                var editlink = $('#calendar').data('editappointmentlink') + '?id=' + calEvent.id;
                window.location = editlink;
            },
            eventResize: function (event, delta, revertFunc, jsEvent, ui, view) {
                var updatelink = $('#calendar').data('updateappointmentlink');
                $.ajax({
                    url: updatelink,
                    method: 'POST',
                    dataType: 'json',
                    data: {
                        id: event.id,
                        years: delta._data.years,
                        months: delta._data.months,
                        days: delta._data.days,
                        starthours: 0,
                        startminutes: 0,
                        endhours: delta._data.hours,
                        endminutes: delta._data.minutes
                    },
                    error: function (data, status) {
                        alert(status.toString());
                    },
                    success: function (data, status) {
                        //debugger;
                    }
                });
            },
            eventDrop: function (event, delta, revertFunc, jsEvent, ui, view) {
                var updatelink = $('#calendar').data('updateappointmentlink');
                $.ajax({
                    url: updatelink,
                    method: 'POST',
                    dataType: 'json',
                    data: {
                        id: event.id,
                        years: delta._data.years,
                        months: delta._data.months,
                        days: delta._data.days,
                        starthours: delta._data.hours,
                        startminutes: delta._data.minutes,
                        endhours: 0,
                        endminutes: 0
                    },
                    error: function (data, status) {
                        alert(status.toString());
                    },
                    success: function (data, status) {
                        //debugger;
                    }
                });
            }
        });
        //// build the locale selector's options
        $.each($.fullCalendar.locales, function (localeCode) {
            $('#locale-selector').append(
                $('<option/>')
                    .attr('value', localeCode)
                    .prop('selected', localeCode === initialLocaleCode)
                    .text(localeCode)
            );
        });

        //// when the selected option changes, dynamically change the calendar option
        $('#locale-selector').on('change', function () {
            if (this.value) {
                $('#calendar').fullCalendar('option', 'locale', this.value);
            }
        });


    }

    $('.fc-prev-button').click(function (e) {
        //var link = $('#calendar').data('indexlink');
        //var date = $('#calendar').fullCalendar('getDate').format();
        //var templink = link + '?datetext=' + date;
        //$('#calendar').fullCalendar('option','events',templink);
        //$('#calendar').fullCalendar('rerenderEvents');
        //$('#calendar').fullCalendar('refetchEvents');
    });

    $('.fc-next-button').click(function (e) {
        //var date = $('#calendar').fullCalendar('getDate').format();
        //$('#calendar').fullCalendar({ events: link + '?date=' + date });
    });
    


});

$(function () {
    $(".draggable").draggable({ revert: true });
    $(".datepicker").datepicker($.datepicker.regional["el"]);
    $(".datepicker").datepicker("option", "changeYear", true);
    $(".datepicker").datepicker("option", "changeMonth", true);
    $(".datepicker").datepicker("option", "dateFormat", "d/m/yy");
});

$(function () {
    $("#measurementsgraphview").dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 400,
        width: 600
    });

    $("#measurementdetailview").dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 600,
        width: 1000
    });

    $("#fooddialog").dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 500,
        width: 900
    });
    $("#mealnotesdialog").dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 180,
        width: 400
    });
    $('#sendbymaildialog').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 480,
        width: 600
    });
    $('#helpcontainer').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 500,
        width: 800
    });
    $('#sendcustomerrecalllink').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 520,
        width: 500
    });

    $('#sendcustomerbasiquestlink').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: 450,
        width: 500
    });
});

$(function () {

    $.widget("custom.catcomplete", $.ui.autocomplete, {
        _create: function () {
            this._super();
            this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
        },
        _renderMenu: function (ul, items) {
            var that = this,
              currentCategory = "";
            $.each(items, function (index, item) {
                var li;
                if (item.category !== currentCategory) {
                    ul.append("<li class='ui-autocomplete-category' style='color:red;'> * " + item.category + "</strong></li>");
                    currentCategory = item.category;
                }
                li = that._renderItemData(ul, item);
                if (item.category) {
                    li.attr("aria-label", item.category + " : " + item.label);
                }
            });
        }
    });

    var url = $('#fooddialog').attr('searchurl');
    var updatefoodlink = $('#fooddialog').attr('updatefooddataurl');
    //Αναζήτηση Food για DietDetail
    $(".searchTerm").catcomplete({
        minLength: 4,
        source: function (request, response) {
            var mealitem = $('#fooddialog').attr('mealid');
            var DietType = $('#fooddialog').attr('diettype');
            $.ajax({
                url: url,
                type: "POST",
                dataType: "json",
                data: { term: request.term, mealtype: mealitem, diettype: DietType },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.GreekName, value: item.GreekName, category: item.FoodCategoryName, item: item };
                    }))
                }
            })
        },
        messages: {
            noResults: "", results: ""
        },
        select: function (event, ui) {
            debugger;
            $('#fooddialog').attr("foodid", ui.item.item.id);
            if (ui.item.item.CanEdit) {
                $('#fooddetails').show();
                $('#fooddetails').load(updatefoodlink + '/' + ui.item.item.id);
            }
            else {
                $('#fooddetails').hide();
            }
        }
    });
});

function changecompany(id, link) {

    $.ajax({
        url: link,
        type: 'POST',
        data: { id: id },
        success: function (data) {
            if (data.Result === "Success") {
                alert(data.Message);
            }
        }
    });
};

function addnewrecipeforemail() {
    var current = $('#currentguids').val();
    var item = $('#newrecipeguid').val();
    var link = $('#attachmentsrender').data('link') + '?current=' + current + '&addrecipe=' + item;
    $('#attachmentsrender').load(link, function () {
        var newcurrent = $('#currentguids').val();
        $('#finalattachments').val(newcurrent);
    });
};

function removerecipeforemail(item) {
    var current = $('#currentguids').val();
    current = current.replace(item, '');
    current = current.replace(';;', ';');
    var link = $('#attachmentsrender').data('link') + '?current=' + current;
    $('#attachmentsrender').load(link, function () {
        var newcurrent = $('#currentguids').val();
        $('#finalattachments').val(newcurrent);
    });

};

function readnotes(mealid, mealindex, dayindex) {
    $('#mealnotesdialog').attr('mealid', mealid);
    $('#mealnotesdialog').attr('mealindex', mealindex);
    $('#mealnotesdialog').attr('dayindex', dayindex);
    var $item = $('#note-' + dayindex + '-' + mealindex);
    var inittext = $item.val();
    $('#mealnotes').val(inittext);
    $('#mealnotesdialog').dialog("open");
};

function savemealnotes() {
    var mealid = $('#mealnotesdialog').attr('mealid');
    var mealindex = $('#mealnotesdialog').attr('mealindex');
    var dayindex = $('#mealnotesdialog').attr('dayindex');
    var newstring = $('#mealnotes').val();
    var $noteitem = $('#note-' + dayindex + '-' + mealindex);
    var updatelink = $('#mealnotesdialog').data('savenotesurl') + '/' + mealid + '?Text=' + newstring.trim();
    //alert(updatelink);
    $.ajax({
        url: updatelink,
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.Result === "Success") {
                $noteitem.val(newstring);
                $('#mealnotesdialog').dialog("close");
            }
            else {
                alert(data.Result);
            }
        }
    })
};

function updatefooddata(b) {
    var id = $('#food-id').val();
    var greekname = $('#food-greekname').val();
    var isbreakfast = $('#food-isbreakfast').is(':checked');
    var equivalent = $('#food-equivalent').val();
    var issnack = $('#food-issnack').is(':checked');
    var islunch = $('#food-islunch').is(':checked');
    var isdinner = $('#food-isdinner').is(':checked');
    var updatelink = $('#fooddialog').attr('updatefooddataurl');
    $.ajax({
        url: updatelink,
        type: "POST",
        dataType: "json",
        data: {
            id: id,
            greekname: greekname,
            equivalent: equivalent,
            isbreakfast: isbreakfast,
            issnack: issnack,
            islunch: islunch,
            isdinner: isdinner
        },
        success: function (data) {
            if (data.Result === "Success") {
                $(b).removeClass('btn-warning', 200).addClass('btn-success', 100);
            }
            else {
                $(b).removeClass('btn-warning', 200).addClass('btn-danger', 100);
            }

        }
    })
};

function showfooddialog(mealid, dayindex, mealindex, foodid, dietdetailid, foodname, mealquantity, mealquantitytype, groupindex) {
    $('#fooddialog').attr('mealid', mealid);
    $('#fooddialog').attr('dayindex', dayindex);
    $('#fooddialog').attr('mealindex', mealindex);
    $('#fooddialog').attr('foodid', foodid);
    $('#fooddialog').attr('dietdetailid', dietdetailid);
    $('#fooditemcontrol').val(foodname);
    $('#foodquantity').val(mealquantity);
    $('#foodquantityname').val(mealquantitytype);
    $('#groupoptionvalue').val(groupindex);
    $('#fooddetails').load($('#fooddialog').attr('updatefooddataurl') + '/' + foodid);

    var title = '';
    switch (mealindex) {
        case 0:
            title = "Πρωϊνό"
            break;
        case 1:
            title = "Πρωϊνό Snack"
            break;
        case 2:
            title = "Γεύμα"
            break;
        case 3:
            title = "Απογευματινό Snack"
            break;
        case 4:
            title = "Δείπνο"
            break;
    }
    title = title + ' ' + dayindex + 'ης ημέρας'
    $('#fooddialog').dialog({
        title: title
    });
    $('#fooddialog').dialog("open");
    $('#foodquantity').select();
};

function removeitemfrommeal(detailid, mealid, dayindex, mealindex) {
    var deletemeallink = $("#fooddialog").attr('deletemeallink') + '/' + detailid;
    var updatelink = $('#fooddialog').attr('updatesingledatemeallink');
    var updatedaytotalslink = $('#fooddialog').attr('updatedaytotalslink') + '&dayindex=' + dayindex;
    $.ajax({
        type: "POST",
        dataType: "json",
        url: deletemeallink,
        success: function (data) {
            if (data.Result === "Success") {
                $('#mealid-' + dayindex + '-' + mealindex).load(updatelink + '/' + mealid);
                $('#day-' + dayindex).load(updatedaytotalslink);
            }
            else {
                alert(data.Result);
            }
        },
        async: true
    });
};

function savemeal() {
    var quantity = ($('#foodquantity').val() === '') ? 1 : $('#foodquantity').val();
    var quantitytype = $('#foodquantityname').val()
    var foodid = $('#fooddialog').attr('foodid');
    var mealid = $('#fooddialog').attr('mealid');
    var dietid = $('#fooddialog').attr('dietid');
    var dayindex = $('#fooddialog').attr('dayindex');
    var mealindex = $('#fooddialog').attr('mealindex');
    var dietdetailid = ($('#fooddialog').attr('dietdetailid') === undefined) ? 0 : $('#fooddialog').attr('dietdetailid');
    var groupindex = $('#groupoptionvalue').val();
    var savelink = $('#fooddialog').attr('savedetailurl');
    var updatedaytotalslink = $('#fooddialog').attr('updatedaytotalslink') + '&dayindex=' + dayindex;
    $.ajax({
        url: savelink,
        type: "POST",
        dataType: "json",
        data: { mealid: mealid, quantity: quantity, quantitydescription: quantitytype, foodid: foodid, groupid: groupindex, dietdetailid: dietdetailid },
        success: function (data) {
            if (data.Result === "Success") {
                $('#fooddialog').dialog("close");
                $('#mealid-' + dayindex + '-' + mealindex).html('');
                $('#mealid-' + dayindex + '-' + mealindex).load($('#fooddialog').attr('updatesingledatemeallink') + '/' + mealid, function () {

                });
                $('#day-' + dayindex).load(updatedaytotalslink);
            }
        }
    })

};

function correctdotandcomma(id) {
    var text = $('#' + id).val();
    text = text.toString().replace(/,/g, '.');
    $('#' + id).val(text);
};

function CreateBMIChart() {
    var $jqueryelemnt = $('#BMICanvas');
    var link = $jqueryelemnt.data('link');
    $.ajax({
        url: link,
        type: "POST",
        dataType: "json",
        data: { customerid: $('#BMICanvas').attr('customerid'), sex: $('#BMICanvas').attr('sex'), width: $jqueryelemnt.innerWidth(), height: $jqueryelemnt.innerHeight() },
        success: function (data) {
            if (data.Result === "Success") {
                var canvas = document.getElementById('BMICanvas');
                var ctx = canvas.getContext("2d");
                ctx.clearRect(0, 0, canvas.clientWidth, canvas.clientHeight);
                ctx.fillStyle = "#f6fed6";
                ctx.fillRect(0, 0, canvas.clientWidth, canvas.clientHeight);
                //Τραβάμε πρώτα τις κάθετες γραμμές του Grid
                jQuery.each(data.VerticalLines, function (index, values) {
                    ctx.beginPath();
                    if (index % 2 === 0) {
                        ctx.strokeStyle = "black";
                        ctx.lineWidth = 1;
                    }
                    else {
                        ctx.strokeStyle = "red";
                        ctx.lineWidth = 0.5;
                    }
                    ctx.moveTo(values[0][0], values[0][1]);
                    ctx.lineTo(values[1][0], values[1][1]);
                    ctx.stroke();
                });
                //Έπειτα τις οριζόντιες γραμμές του grid
                jQuery.each(data.HorizontalLines, function (index, values) {
                    ctx.beginPath();
                    if (index % 2 === 0) {
                        ctx.strokeStyle = "black";
                        ctx.lineWidth = 0.5;
                    }
                    else {
                        ctx.strokeStyle = "blue";
                        ctx.lineWidth = 0.8;
                    }
                    ctx.moveTo(values[0][0], values[0][1]);
                    ctx.lineTo(values[1][0], values[1][1]);
                    ctx.stroke();
                });
                //Έπειτα τα Text των ηλικιών
                jQuery.each(data.AgesScale, function (index, values) {
                    ctx.beginPath();
                    ctx.font = "10px Comic Sans MS";
                    ctx.fillStyle = "red";
                    ctx.textAlign = "center";
                    ctx.fillText(values.Text, values.LocationX, values.LocationY);
                    ctx.stroke();
                });
                //Έπειτα τα Text των BMI
                jQuery.each(data.BMIScale, function (index, values) {
                    ctx.beginPath();
                    ctx.font = "10px Comic Sans MS";
                    ctx.fillStyle = "red";
                    ctx.textAlign = "center";
                    ctx.fillText(values.Text, values.LocationX, values.LocationY);
                    ctx.stroke();
                });
                //Έπειτα τις γραμμές των BMI
                jQuery.each(data.LineValues, function (index, values) {
                    ctx.beginPath();
                    ctx.strokeStyle = values.Color;
                    ctx.setLineDash(values.DashArray);
                    ctx.lineWidth = values.Width;
                    ctx.moveTo(values.Values[0][0], values.Values[0][1]);
                    for (i = 1; i < values.Values.length; i++) {
                        ctx.lineTo(values.Values[i][0], values.Values[i][1]);
                    }
                    ctx.stroke();
                });
                if (data.CustomerLine !== null) {
                    ctx.beginPath();
                    ctx.strokeStyle = "red";
                    //ctx.lineWidth = 4;
                    //ctx.moveTo(data.CustomerLine[0][0], data.CustomerLine[0][1]);
                    //ctx.lineTo(data.CustomerLine[0][0], data.CustomerLine[0][1]);
                    ctx.arc(data.CustomerLine[0][0], data.CustomerLine[0][1], 1, 0, 2 * Math.PI);
                    ctx.stroke();
                    for (i = 1; i < data.CustomerLine.length; i++) {
                        ctx.lineTo(data.CustomerLine[i][0], data.CustomerLine[i][1]);
                    }

                    ctx.stroke();
                }

            }
        }
    });
}
