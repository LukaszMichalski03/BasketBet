// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener('scroll', function () {
    var navbar = document.getElementById('navBar'); // Zastąp 'yourNavbarId' rzeczywistym identyfikatorem nawigacji

    if (window.scrollY > 0) {
        navbar.classList.add('navbar-scrolled');
    } else {
        navbar.classList.remove('navbar-scrolled');
    }
});
///////////////////////////////////////////////////////////////////////
var betsList = []; // Inicjalizacja pustej listy zakładów
function toggleBet(match, teamPosition, button) {
    var matchObject = JSON.parse(match);
    var isActive = button.classList.contains('active');

    // Sprawdź, czy zakład już istnieje w liście
    var isBetAlreadyAdded = betsList.some(function (bet) {
        return bet.GameVM.HomeTeamVM.Name === matchObject.HomeTeamVM.Name &&
            bet.GameVM.AwayTeamVM.Name === matchObject.AwayTeamVM.Name &&
            bet.TeamTypedOn === teamPosition;
    });

    if (!isActive && !isBetAlreadyAdded) {
        // Dodaj zakład tylko jeśli nie istnieje w liście i przycisk nie jest aktywny
        addBet(match, teamPosition);
        button.classList.add('active'); // Dodaj klasę aktywną, aby oznaczyć, że przycisk jest aktywny
    } else if (isActive && isBetAlreadyAdded) {
        // Usuń zakład tylko jeśli istnieje w liście i przycisk jest aktywny
        var existingBetIndex = betsList.findIndex(function (bet) {
            return bet.GameVM.HomeTeamVM.Name === matchObject.HomeTeamVM.Name &&
                bet.GameVM.AwayTeamVM.Name === matchObject.AwayTeamVM.Name &&
                bet.TeamTypedOn === teamPosition;
        });
        if (existingBetIndex !== -1) {
            betsList.splice(existingBetIndex, 1);
        }
        button.classList.remove('active'); // Usuń klasę aktywną, aby oznaczyć, że przycisk nie jest już aktywny

        // Usuń odpowiednie elementy HTML z kontenera
        var betContainer = document.getElementById('betContainerOverflow');
        var betsToRemove = betContainer.querySelectorAll('.flex-container-blue span');
        betsToRemove.forEach(function (bet) {
            if (bet.innerText === matchObject.HomeTeamVM.Name + ' - ' + matchObject.AwayTeamVM.Name) {
                bet.parentElement.parentElement.remove();
            }
        });
    }
}






function addBet(match, teamPosition) {
    var matchObject = JSON.parse(match);

    // Teraz matchObject powinien być obiektem JavaScript zawierającym właściwość HomeTeamVM
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
            return; // Zakończ funkcję, jeśli wartość teamPosition jest nieprawidłowa
        }

        // Tworzymy nowy obiekt SingleGameBet
        var singleGameBet = {
            Id: 0, // Jeśli potrzebujesz przypisać rzeczywisty identyfikator, tutaj go ustaw
            GameVM: {
                HomeTeamVM: matchObject.HomeTeamVM,
                AwayTeamVM: matchObject.AwayTeamVM,
                Date: matchObject.Date // Ustawiamy datę z meczu
            },
            TeamTypedOn: teamPosition === "Home" ? "Home" : "Away",
            Course: course // Ustawiamy kurs na podstawie wartości OddsHomeTeam lub OddsAwayTeam
        };

        // Dodajemy nowy obiekt SingleGameBet do listy betsList
        betsList.push(singleGameBet);

        // Tworzymy nowy element div dla zakładu
        var betElement = document.createElement('div');
        betElement.classList.add('flex-container-blue');

        // Tworzymy nagłówek
        var header = document.createElement('div');
        header.classList.add('fl-con-header');
        header.innerHTML = '<span>' + homeTeamName + ' - ' + awayTeamName + '</span>' +
            '<img src="/icons/bin.png" alt="" width="32px" height="32px" onclick="removeBet(' + (betsList.length - 1) + ')" />';

        betElement.appendChild(header);

        // Tworzymy zawartość zakładu
        var content = document.createElement('div');
        content.classList.add('fl-con-content');
        content.innerHTML = '<div class="content-winners-name">' +
            '<p>' + teamObject.Name + '</p>' +
            '<span>Game Winner</span>' +
            '</div>' +
            '<div class="content-single-course">' +
            '<p>' + course + '</p>' + // Wstawiamy kurs do zawartości zakładu
            '</div>';
        betElement.appendChild(content);

        // Dodajemy nowy element do kontenera
        var betContainer = document.getElementById('betContainerOverflow');
        betContainer.appendChild(betElement);
    } else {
        console.error("Brak właściwości HomeTeamVM w obiekcie match");
    }
}

function removeBet(index) {
    // Usuń element z listy betsList
    betsList.splice(index, 1);

    // Usuń odpowiadający element HTML z kontenera
    var betContainer = document.getElementById('betContainerOverflow');
    betContainer.removeChild(betContainer.childNodes[index]);
}





