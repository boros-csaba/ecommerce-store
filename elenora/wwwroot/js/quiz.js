var answers = [];
var quizId = 0;

function initializeQuiz() {
    $(".quiz").html("");
    var innerContainer = $('<div class="quiz-inner"></div>');
    innerContainer.append($('<div class="quiz-title">Tudd meg 6 lépésben, hogy melyik ásvány való neked!</div>'));
    var startButton = $('<div class="quiz-start">Kezdés</div>');
    startButton.click(function () { startQuiz(); });
    innerContainer.append(startButton);
    $(".quiz").append(innerContainer);
}

function startQuiz() {
    $(".quiz-start").remove();
    $.ajax({
        url: "/quiz/start",
        data: { quizName: "beadType" },
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            quizId = data;
            console.log(quizId);
            var statusInducators = $('<div class="quiz-status"></div>');
            for (var i = 1; i < 7; i++) {
                statusInducators.append('<span class="quiz-status-' + i + '">' + i + '</span>')
            }
            statusInducators.insertBefore($(".quiz-inner"));
            $(".quiz-inner").append('<div class="quiz-answers"></div>');
            $(".quiz-title").addClass("quiz-question").removeClass("quiz-title");
            completeQuestion(0, "start");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

function completeQuestion(questionNr, answer) {
    if (questionNr > 0) {
        answers[questionNr - 1] = answer;
        $.ajax({
            url: "/quiz/save",
            method: "POST",
            data: {
                quizId: quizId,
                answerCount: questionNr - 1,
                answer: answer
            },
            success: function (_data, _textStatus, _jqXHR) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log("Error");
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        });
    }
    var nextQuestion = getQuestion(questionNr, answer);
    if (nextQuestion) {
        $(".quiz-question").html(nextQuestion.question);
        $(".quiz-answers").html("");
        for (var i = 0; i < nextQuestion.answers.length; i++) {
            var answer = $('<span>' + nextQuestion.answers[i] + '</span>');
            if (nextQuestion.answers.length > 7) {
                answer.css("width", "50%");
            }
            answer.click(function () { completeQuestion(questionNr + 1, $(this).html()); });
            $(".quiz-answers").append(answer);
        }
    }
    else {
        window.dataLayer = window.dataLayer || [];
        window.dataLayer.push({
            event: 'quiz_completed'
        });

        $(".quiz-inner").html("");
        $(".quiz-status").css("display", "none");
        $(".quiz-inner").css("background", "white");
        $(".quiz-inner").append('<span class="quiz-done">Ezeket az ásványokat ajánljuk neked!</div>');
        var resultBeads = getResultBeads();
        for (var i = 0; i < resultBeads.length; i++) {
            var beadContainer = $('<div class="quiz-result-bead"></div>');
            var titleContainer = $('<div class="quiz-result-title"></div>');
            titleContainer.append($('<span class="quiz-result-' + (i + 1) + '">#' + (i + 1) + '</span>'));
            var beadHeader = $('<div class="quiz-bead-header"></div>');
            var picture = $('<picture></picture>');
            picture.append($('<source srcset="/images/components/' + resultBeads[i].url + '.webp" type="image/webp">'));
            picture.append($('<source srcset="/images/components/' + resultBeads[i].url + '.png" type="image/png">'));
            picture.append($('<img src="/images/components/' + resultBeads[i].url + '.png">'));
            beadHeader.append(picture);
            beadHeader.append($('<span class="quiz-result-bead-name">' + resultBeads[i].name + '</span>'));
            titleContainer.append(beadHeader);
            beadContainer.append(titleContainer);
            beadContainer.append($('<span class="quiz-result-bead-description">' + resultBeads[i].description.replace(/NewLine/g, "<br />") + '</span>'))
            $(".quiz-inner").append(beadContainer);
        }

        $.ajax({
            url: "/quiz/save-result",
            method: "POST",
            data: {
                quizId: quizId,
                result: resultBeads[0].url + ", " + resultBeads[1].url + ", " + resultBeads[2].url
            },
            success: function (_data, _textStatus, _jqXHR) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log("Error");
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        });
    }
    $(".quiz-status-" + (questionNr + 1)).addClass("question-status-active");
}

function getQuestion(currentQuestion) {
    if (currentQuestion == 0) {
        return {
            question: "1. Mi a nemed?",
            answers: ["Nő", "Férfi"]
        };
    }
    else if (currentQuestion == 1) {
        return {
            question: "2. Hány éves vagy?",
            answers: ["18 alatt", "18-30", "31-45", "46-65", "65 felett"]
        };
    }
    else if (currentQuestion == 2) {
        return {
            question: "3. Mi a fontosabb neked?",
            answers: [
                "Az ásványok jótékony hatása",
                "Legyen divatos és szép",
                "Horoszkópommal legyen összhangban"]
        };
    }
    else if (currentQuestion == 3) {
        if (answers[2] == "Az ásványok jótékony hatása") {
            return {
                question: "4. Melyik területet erősítenéd?",
                answers: [
                    "Szerelem",
                    "Pénz",
                    "Lélek",
                    "Egészség"]
            };
        }
        else if (answers[2] == "Legyen divatos és szép") {
            return {
                question: "4. Mennyire rajongsz az ékszerekért?",
                answers: [
                    "Minden ruhámhoz más-más ékszert hordok",
                    "Megvan a kedvencem ékszerem amit mindig hordok",
                    "Csak alkalmanként hordok ékszert"]
            };
        }
        else if (answers[2] == "Horoszkópommal legyen összhangban") {
            return {
                question: "4. Mi a csillagjegyed?",
                answers: ["Kos", "Bika", "Ikrek", "Rák", "Oroszlán", "Szűz", "Mérleg", "Skorpió", "Nyilas", "Bak", "Vízöntő", "Halak"]
            };
        }
    }
    else if (currentQuestion == 4) {
        if (answers[3] == "Szerelem") {
            return {
                question: "5. Melyik hatást választanád?",
                answers: [
                    "Romantikusság",
                    "Szerelmi bánat enyhítése",
                    "Kapcsolat elmélyítése",
                    "Fogyás",
                    "Termékenység növelése"]
            };
        }
        else if (answers[3] == "Pénz") {
            return {
                question: "5. Melyik hatást választanád?",
                answers: [
                    "Szerencse",
                    "Kitartás, erő",
                    "Bölcsesség",
                    "Kreativitás",]
            };
        }
        else if (answers[3] == "Lélek") {
            return {
                question: "5. Melyik hatást választanád?",
                answers: [
                    "Önbizalom",
                    "Kitartás, erő",
                    "Megbocsátás",
                    "Pozitivitás"]
            };
        }
        else if (answers[3] == "Egészség") {
            return {
                question: "5. Melyik hatást választanád?",
                answers: [
                    "Gyomor problémák enyhítése",
                    "Fájdalom csillapítás",
                    "Termékenység növelése",
                    "Szív és keringési zavarok enyhítése",
                    "Fogyás"]
            };
        }
        else if (answers[2] == "Legyen divatos és szép") {
            return {
                question: "5. Milyen a stílusod?",
                answers: [
                    "Inkább elegánsan szeretek öltözködni",
                    "A sportos ruhákat szeretem inkább",
                    "Szeretem a színes szetteket"]
            };
        }
        else if (answers[2] == "Horoszkópommal legyen összhangban") {
            return {
                question: "5. Melyik területet erősítenéd?",
                answers: [
                    "Szerelem",
                    "Pénz",
                    "Egészség"]
            };
        }
    }
    else if (currentQuestion == 5) {
        return {
            question: "6. Melyik igaz rád",
            answers: [
                "Visszahúzódó típus vagyok",
                "Szeretek kitűnni a tömegből",
                "Egyik sem"]
        };
    }
}

function getResultBeads() {
    var beadNames = [];
    beadNames = ["rozsakvarc", "howlit", "zebra-jaspis"];
    var result = [];
    for (var i = 0; i < beadNames.length; i++) {
        result.push(quizBeads.filter(b => b.url == beadNames[i])[0]);
    }
    return result;
}