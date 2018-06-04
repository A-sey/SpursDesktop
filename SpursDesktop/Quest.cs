using System;

namespace SpursDesktop
{
    class Quest
    {
        String text;
        String answer = "";
        String image = "";
        public Quest() { text = answer = image = ""; }
        public Quest(String str)
        {
            // Делим строку на ячейки (должно получиться 2 штуки)
            String[] que = str.Split(new String[] { "<td>", "</td>" }, StringSplitOptions.RemoveEmptyEntries);
            // Заносим вопрос
            text = que[0];
            // Пересохрняем ответы
            String temp = que[1];
            // Пока есть строки
            while (temp.IndexOf("<p>") != -1)
            {
                // Добавляем в список первый подвернувшийся ответ
                answer += temp.Substring(temp.IndexOf("<p>") + 3, temp.IndexOf("</p>") - temp.IndexOf("<p>") - 3) + "\n";
                // Удаляем весь текст с начала по конец этого ответа
                temp = temp.Remove(0, temp.IndexOf("</p>") + 4);
            }
            // Восстанавливаем строку с ответами
            temp = que[1];
            // Пока есть ссылки на картинки
            while (temp.IndexOf("src=\"") != -1)
            {
                // Находим начало адреса картинки
                int start = temp.IndexOf("src=\"") + 5;
                // Добавляем адрес картинки в список
                image += temp.Substring(start, temp.IndexOf("\"", start) - start) + "\n";
                // Удаляем текст с начала по конец адреса картинки
                temp = temp.Remove(0, temp.IndexOf("\"", start) + 1);
            }
        }

        public String GetQuestion() { return text; }
        public String GetAnswer() { return answer; }
        public String GetImage() { return image; }

        public String GetHTML()
        {
            // Вносим текст вопроса в отдельную ячейку
            String htext = "<td>" + text.Replace("\r\n"," ") + "</td>";
            if (htext == "<td></td>")
                htext = "<td> </td>";
            // Создаём пустую строку для текста ответа
            String hanswer = "";
            // Разбиваем текст ответа на строки
            String[] ansParts = answer.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            // Каждую строку заносим отдельно в переменную
            foreach (String i in ansParts)
            {
                hanswer += "<p>" + i + "</p>";
            }
            // Повторяем для картинок
            String[] imgParts = image.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String i in imgParts)
            {
                hanswer += "<img src=\""+i+"\"></img>";
            }
            // Говорим, что получили новую ячейку
            hanswer = "<td>" + hanswer + "</td>";
            if (hanswer == "<td></td>")
                hanswer = "<td> </td>";
            // И возвращаем две ячейки в виде строки
            return "<tr>" + htext + hanswer + "</tr>";
        }

        public void SetQuestion (String que) { text = que; }
        public void SetAnswer(String ans) { answer = ans; }
        public void SetImage(String img) { image = img; }
    }
}