
$(document).ready(function () {
    $('#searchGroupsForm').on('submit', function (e) {
        e.preventDefault();
        const formData = $(this).serialize();
        
        $.post('/Queries/SearchGroups', formData)
            .done(function (data) {
                $('#groupsResult').html(data);
            })
            .fail(function () {
                $('#groupsResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
            });
    });
});

function calculateCost(calculationType) {
    const languageId = $('#costLanguageSelect').val();
    
    if ((calculationType === 'language' || calculationType === 'byLevel' || calculationType === 'monthly') && !languageId) {
        alert('Оберіть мову для розрахунку');
        return;
    }
    
    $.post('/Queries/CalculateCost', { calculationType: calculationType, languageId: languageId })
        .done(function (response) {
            if (response.success) {
                displayCostResult(response.data, calculationType);
            } else {
                $('#costResult').html('<div class="alert alert-danger">Помилка: ' + response.error + '</div>');
            }
        })
        .fail(function () {
            $('#costResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function displayCostResult(data, type) {
    let html = '<div class="alert alert-success">';
    
    if (type === 'all') {
        html += '<h6>Вартість всіх мов:</h6>';
        for (const [language, cost] of Object.entries(data)) {
            html += `<p><strong>${language}:</strong> ${cost.toFixed(2)} грн</p>`;
        }
    } else if (type === 'language') {
        html += `<h6>Повна вартість:</h6><p class="h4">${data.toFixed(2)} грн</p>`;
    } else if (type === 'byLevel') {
        html += '<h6>Вартість за рівнями:</h6>';
        for (const [level, cost] of Object.entries(data)) {
            html += `<p><strong>${level}:</strong> ${cost.toFixed(2)} грн</p>`;
        }
    } else if (type === 'monthly') {
        html += `<h6>Місячна вартість:</h6><p class="h4">${data.toFixed(2)} грн</p>`;
    }
    
    html += '</div>';
    $('#costResult').html(html);
}

function getFailedExams() {
    $.post('/Queries/GetFailedExams')
        .done(function (response) {
            if (response.success) {
                displayFailedExamsResult(response.data);
            } else {
                $('#failedExamsResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
            }
        })
        .fail(function () {
            $('#failedExamsResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function displayFailedExamsResult(data) {
    let html = `<div class="alert alert-info"><h6>Загальна кількість нездач: ${data.totalCount}</h6></div>`;
    
    if (data.failedResults && data.failedResults.length > 0) {
        html += '<div class="table-responsive"><table class="table table-striped"><thead><tr><th>Слухач</th><th>Мова</th><th>Рівень</th><th>Оцінка</th></tr></thead><tbody>';
        
        data.failedResults.forEach(function (result) {
            html += `<tr><td>${result.student.fullName}</td><td>${result.exam.level.language.name}</td><td>${result.exam.level.name}</td><td><span class="badge bg-danger">${result.grade}</span></td></tr>`;
        });
        
        html += '</tbody></table></div>';
    } else {
        html += '<p>Незданих іспитів немає.</p>';
    }
    
    $('#failedExamsResult').html(html);
}

function getTeachersByLanguageCount() {
    $.post('/Queries/GetTeachersByLanguageCount')
        .done(function (response) {
            if (response.success) {
                displayTeachersResult(response.data);
            }
        })
        .fail(function () {
            $('#teachersResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function displayTeachersResult(data) {
    let html = '<div class="row">';
    
    const categories = [
        { key: '1', title: 'Одна мова', class: 'primary' },
        { key: '2', title: 'Дві мови', class: 'success' },
        { key: '3', title: 'Три+ мови', class: 'info' }
    ];
    
    categories.forEach(function (category) {
        html += `<div class="col-md-4"><div class="card"><div class="card-header bg-${category.class} text-white"><h6>${category.title}</h6></div><div class="card-body">`;
        
        if (data[category.key] && data[category.key].length > 0) {
            html += '<ul class="list-group list-group-flush">';
            data[category.key].forEach(function (teacher) {
                html += `<li class="list-group-item">${teacher.fullName}</li>`;
            });
            html += '</ul>';
        } else {
            html += '<p class="text-muted">Викладачів немає</p>';
        }
        
        html += '</div></div></div>';
    });
    
    html += '</div>';
    $('#teachersResult').html(html);
}

function getPaymentStatus(statusType) {
    $.post('/Queries/GetPaymentStatus', { statusType: statusType })
        .done(function (data) {
            $('#paymentStatusResult').html(data);
        })
        .fail(function () {
            $('#paymentStatusResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function getStudentsByLanguage() {
    $.post('/Queries/GetStudentsByLanguage')
        .done(function (response) {
            if (response.success) {
                displayStudentsByLanguageResult(response.data);
            }
        })
        .fail(function () {
            $('#studentsByLanguageResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function displayStudentsByLanguageResult(data) {
    let html = '<div class="row">';
    
    html += '<div class="col-md-6"><div class="card"><div class="card-header bg-primary text-white"><h6>Вивчають Німецьку</h6></div><div class="card-body">';
    if (data.germanStudents && data.germanStudents.length > 0) {
        html += '<ul class="list-group list-group-flush">';
        data.germanStudents.forEach(function (student) {
            html += `<li class="list-group-item">${student.fullName} - ${student.email || 'Немає email'}</li>`;
        });
        html += '</ul>';
    } else {
        html += '<p class="text-muted">Студентів немає</p>';
    }
    html += '</div></div></div>';
    
    html += '<div class="col-md-6"><div class="card"><div class="card-header bg-success text-white"><h6>Вивчають > 1 мови</h6></div><div class="card-body">';
    if (data.multiLanguageStudents && data.multiLanguageStudents.length > 0) {
        html += '<ul class="list-group list-group-flush">';
        data.multiLanguageStudents.forEach(function (student) {
            html += `<li class="list-group-item">${student.fullName} - ${student.email || 'Немає email'}</li>`;
        });
        html += '</ul>';
    } else {
        html += '<p class="text-muted">Студентів немає</p>';
    }
    html += '</div></div></div>';
    
    html += '</div>';
    $('#studentsByLanguageResult').html(html);
}

function applySmallGroupSurcharge() {
    if (!confirm('Ви впевнені, що хочете застосувати надбавку 20% для малих груп?')) {
        return;
    }
    
    $.post('/Queries/ApplySmallGroupSurcharge')
        .done(function (response) {
            if (response.success) {
                let html = `<div class="alert alert-success">${response.message}</div>`;
                if (response.groups && response.groups.length > 0) {
                    html += displayGroupsTable(response.groups, 'Малі групи');
                }
                $('#smallGroupResult').html(html);
            } else {
                $('#smallGroupResult').html(`<div class="alert alert-danger">Помилка: ${response.error}</div>`);
            }
        })
        .fail(function () {
            $('#smallGroupResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

function applyLargeGroupDiscount() {
    if (!confirm('Ви впевнені, що хочете застосувати знижку 5% для великих груп?')) {
        return;
    }
    
    $.post('/Queries/ApplyLargeGroupDiscount')
        .done(function (response) {
            if (response.success) {
                let html = `<div class="alert alert-success">${response.message}</div>`;
                if (response.groups && response.groups.length > 0) {
                    html += displayGroupsTable(response.groups, 'Великі групи');
                }
                $('#largeGroupResult').html(html);
            } else {
                $('#largeGroupResult').html(`<div class="alert alert-danger">Помилка: ${response.error}</div>`);
            }
        })
        .fail(function () {
            $('#largeGroupResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
        });
}

$(document).ready(function () {
    $('#scheduleForm').on('submit', function (e) {
        e.preventDefault();
        const formData = $(this).serialize();
        
        $.post('/Queries/GetSchedule', formData)
            .done(function (data) {
                $('#scheduleResult').html(data);
            })
            .fail(function () {
                $('#scheduleResult').html('<div class="alert alert-danger">Помилка при завантаженні даних</div>');
            });
    });
});

function displayGroupsTable(groups, title) {
    let html = `<h6>${title}:</h6><div class="table-responsive"><table class="table table-striped table-sm"><thead><tr><th>Група</th><th>Рівень</th><th>Кількість студентів</th></tr></thead><tbody>`;
    
    groups.forEach(function (group) {
        const studentCount = group.enrollments ? group.enrollments.length : 0;
        html += `<tr><td>${group.groupName}</td><td>${group.level ? group.level.name : 'N/A'}</td><td>${studentCount}</td></tr>`;
    });
    
    html += '</tbody></table></div>';
    return html;
}