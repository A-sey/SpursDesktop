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
            // Преобразуем строку
            temp = temp.Replace("</p>", "");
            temp = temp.Replace("\"></img>", "");
            temp = temp.Replace("<img src=\"", "<p>:i:");
            // Пока есть строки
            String[] parts = temp.Split(new String[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String i in parts)
            {
                answer += i + "\n";
                if (i.StartsWith(":i:"))
                    image += i.Remove(0,3) + "\n";
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
            // Разбиваем список картинок на строки
            String[] imgParts = image.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            // Каждую строку заносим отдельно в переменную
            foreach (String i in ansParts)
            {
                if (!i.StartsWith(":i:"))
                hanswer += "<p>" + i + "</p>";
                else
                for(int im=0; im<imgParts.Length; im++)
                    if (imgParts[im].Contains(i.Remove(0, 3)))
                        {
                            hanswer += "<img src=\"" + imgParts[im] + "\"></img>";
                            imgParts[im] = "";
                        }
            }
            // Вносим картинки, которые не упоминались отдельно
            foreach (String i in imgParts)
            {
                if (i!="")
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