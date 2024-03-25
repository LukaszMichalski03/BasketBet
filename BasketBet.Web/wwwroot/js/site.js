// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener('scroll', function () {
    var navbar = document.getElementById('navBar');

    if (window.scrollY > 0) {
        navbar.classList.add('navbar-scrolled');
    } else {
        navbar.classList.remove('navbar-scrolled');
    }
});



var betsList = [];
function toggleBet(matchData, betType, button) {
    var matchObject = JSON.parse(matchData);
    var isActive = button.classList.contains('active');

    if (isActive) {
        button.classList.remove('active');

        // Usuń elementy z betsList, które pasują do kryteriów
        betsList = betsList.filter(function (bet) {
            return !(bet.GameVMId === matchObject.Id);
        });

        // Usuń dzieci z elementu betContainerOverflow, które pasują do kryteriów
        var betContainer = document.getElementById('betContainerOverflow');
        var betsToRemove = betContainer.querySelectorAll('.flex-container-blue span');
        betsToRemove.forEach(function (bet) {
            if (bet.innerText === matchObject.HomeTeamVM.Name + ' - ' + matchObject.AwayTeamVM.Name) {
                bet.parentElement.parentElement.remove();
            }
        });
    } else {
        // Sprawdź, czy w betsList nie ma żadnego elementu o GameVMId równym matchObject.Id
        var isBetAlreadyAdded = betsList.some(function (bet) {
            return bet.GameVMId === matchObject.Id;
        });

        // Jeśli nie ma żadnego elementu o takim GameVMId, dodaj 'active' do przycisku
        // i dodaj zakład do listy betsList
        if (!isBetAlreadyAdded) {
            button.classList.add('active');
            addBet(matchData, betType);
        }
    }

    updateTotalCourse();
    updateBetsCount();
}


function addBet(match, teamPosition) {
    var matchObject = JSON.parse(match);

    if (matchObject.HomeTeamVM) {
        var homeTeamName = matchObject.HomeTeamVM.Name;
        var awayTeamName = matchObject.AwayTeamVM.Name;

        var teamObject;
        var course;

        if (teamPosition === "Home") {
            teamObject = matchObject.HomeTeamVM;
            course = matchObject.OddsHomeTeam;
        } else if (teamPosition === "Away") {
            teamObject = matchObject.AwayTeamVM;
            course = matchObject.OddsAwayTeam;
        } else {
            console.error("Nieprawidłowa wartość parametru teamPosition. Musi być 'Home' lub 'Away'.");
            return;
        }

        var singleGameBet = {
            GameVMId: matchObject.Id,
            GameVM: {
                Id: matchObject.Id,
                HomeTeamVMId: matchObject.HomeTeamVMId,
                AwayTeamVMId: matchObject.AwayTeamVMId,
                HomeTeamVM: matchObject.HomeTeamVM,
                AwayTeamVM: matchObject.AwayTeamVM,
                Date: matchObject.Date,
                OddsAwayTeam: matchObject.OddsAwayTeam,
                OddsHomeTeam: matchObject.OddsHomeTeam
            },
            TeamTypedOn: teamPosition === "Home" ? matchObject.HomeTeamVM : matchObject.AwayTeamVM,
            TeamTypedOnId: teamPosition === "Home" ? matchObject.HomeTeamVMId : matchObject.AwayTeamVMId,
            Course: course
        };

        betsList.push(singleGameBet);

        var betElement = document.createElement('div');
        betElement.classList.add('flex-container-blue');

        var header = document.createElement('div');
        header.classList.add('fl-con-header');
        header.innerHTML = '<span>' + homeTeamName + ' - ' + awayTeamName + '</span>';

        betElement.appendChild(header);

        var content = document.createElement('div');
        content.classList.add('fl-con-content');
        content.innerHTML = '<div class="content-winners-name">' +
            '<p>' + teamObject.Name + '</p>' +
            '<span>Game Winner</span>' +
            '</div>' +
            '<div class="content-single-course">' +
            '<p>' + course + '</p>' +
            '</div>';
        betElement.appendChild(content);

        var betContainer = document.getElementById('betContainerOverflow');
        betContainer.appendChild(betElement);
        updateTotalCourse();
        updateBetsCount();
    } else {
        console.error("Brak właściwości HomeTeamVM w obiekcie match");
    }
}


function removeAllBets() {

    betsList = [];

    updateTotalCourse();
    updateBetsCount();

    var betContainer = document.getElementById('betContainerOverflow');
    while (betContainer.firstChild) {
        betContainer.removeChild(betContainer.firstChild);
    }

    var buttons = document.querySelectorAll('.btn.white-btn');

    buttons.forEach(function (button) {
        button.classList.remove('active');
    });
}
function updateTotalCourse() {
    var totalCourseElement = document.querySelector('.totalcourse');
    var totalCourseValue = 1;

    betsList.forEach(function (bet) {
        totalCourseValue *= bet.Course;
    });

    totalCourseElement.textContent = totalCourseValue.toFixed(2);
}
function updateBetsCount() {
    var countBetsElement = document.getElementById('count-bets');
    var betsCount = betsList.length; // Załóżmy, że betsList jest dostępna
    if (betsCount == 1) countBetsElement.textContent = betsCount + ' Bet';
    else countBetsElement.textContent = betsCount + ' Bets';
    
}
document.addEventListener('DOMContentLoaded', function () {
    var pointsInput = document.querySelector('.con-footer-row-left input[type="number"]');
    var totalCourseElement = document.querySelector('.totalcourse');
    var totalWinningElement = document.querySelector('.total-winning');

    totalCourseElement.addEventListener('DOMSubtreeModified', function () {
        updateTotalWinning();
        updateBetsCount();
    });

    pointsInput.addEventListener('input', function () {
        updateTotalWinning();
        
    });

    function updateTotalWinning() {
        var pointsValue = parseFloat(pointsInput.value);
        var totalCourseValue = parseFloat(totalCourseElement.textContent);

        var totalWinning;

        if (isNaN(pointsValue)) {
            totalWinning = 0;
        } else {
            totalWinning = pointsValue * totalCourseValue;
            totalWinning = totalWinning.toFixed(2);
        }

        totalWinningElement.textContent = totalWinning + ' points';
    }


});

function sendBetsToController() {
    var pointsInput = document.querySelector('input[type="number"]');
    var totalWinningElement = document.querySelector('.total-winning');
    var totalWinningText = totalWinningElement.textContent.trim();
    totalWinningText = totalWinningText.slice(0, -7);
    var totalWinningValue = parseFloat(totalWinningText);

    var totalCourseElement = document.querySelector('.totalcourse');

    var pointsValue = Number(pointsInput.value);
    var totalCourseValue = Number(totalCourseElement.textContent);

    if (pointsValue > 0 && !isNaN(totalWinningValue) && !isNaN(pointsValue)) {
        var dataToSend = {
            BetsList: betsList,
            TotalCourse: totalCourseValue,
            Points: pointsValue,
            TotalWinning: totalWinningValue
        };

        fetch('/Home/CreateBet', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dataToSend)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                // Obsłuż odpowiedź z serwera
                if (data.success) {
                    // Pobierz wynik z odpowiedzi i przekaż go do akcji NewBet
                    var result = data.result;
                    window.location.href = '/Home/NewBet?BetId=' + result;
                } else {
                    console.log('Wystąpił błąd podczas przetwarzania żądania.');
                }
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
    } else {
        console.log('Wartość punktów musi być większa niż 0 lub niepoprawne wartości dla totalWinningValue lub pointsValue.');
    }
}

async function claimPoints() {
    try {
        const response = await fetch('/Home/ClaimPoints', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            console.log('Punkty zostały pomyślnie odebrane.');
            // Odśwież stronę po odebraniu nagrody
            location.reload();
        } else {
            console.error('Nie udało się odebrać punktów.');
        }
    } catch (error) {
        console.error('Wystąpił błąd:', error);
    }
}





