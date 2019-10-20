WITH cte AS ( 
SELECT title,row_num
FROM (
	SELECT 
		ROW_NUMBER() OVER ( PARTITION BY TITLE ORDER BY TITLE) row_num,
		*
     FROM [FaceRecognition].[dbo].[Film]) TAB
	WHERE row_num > 1)

DELETE FROM cte
WHERE row_num > 1;

 DELETE FROM [FaceRecognition].[dbo].[Film]
 WHERE title like '"%' ;

