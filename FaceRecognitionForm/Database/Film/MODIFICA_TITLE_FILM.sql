UPDATE Film
SET Title = SUBSTRING(Title, 0, (LEN(Title)-1))