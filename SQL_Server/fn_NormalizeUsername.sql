USE [UserReview]
GO

-- ============================================================================
-- fn_NormalizeUsername
--
-- Normalizes usernames from source data to A–Z, a–z, 0–9 only.
-- Accented characters (e.g. ï, é, ñ) are converted to their ASCII equivalents
-- (i, e, n). Any other non-alphanumeric characters are removed.
--
-- Examples: "esakaïr" -> "esakair", "José" -> "Jose"
-- ============================================================================

CREATE OR ALTER FUNCTION dbo.fn_NormalizeUsername (@Input NVARCHAR(255))
RETURNS VARCHAR(255)
AS
BEGIN
    IF @Input IS NULL RETURN NULL;

    DECLARE @Result NVARCHAR(255) = N'';
    DECLARE @Len INT = LEN(@Input);
    DECLARE @i INT = 1;
    DECLARE @c NCHAR(1);
    DECLARE @u INT;

    -- Step 1: replace common accented characters with ASCII equivalents
    SET @Input = REPLACE(@Input, N'ï', N'i');
    SET @Input = REPLACE(@Input, N'í', N'i');
    SET @Input = REPLACE(@Input, N'ì', N'i');
    SET @Input = REPLACE(@Input, N'î', N'i');
    SET @Input = REPLACE(@Input, N'é', N'e');
    SET @Input = REPLACE(@Input, N'è', N'e');
    SET @Input = REPLACE(@Input, N'ê', N'e');
    SET @Input = REPLACE(@Input, N'ë', N'e');
    SET @Input = REPLACE(@Input, N'á', N'a');
    SET @Input = REPLACE(@Input, N'à', N'a');
    SET @Input = REPLACE(@Input, N'â', N'a');
    SET @Input = REPLACE(@Input, N'ä', N'a');
    SET @Input = REPLACE(@Input, N'ã', N'a');
    SET @Input = REPLACE(@Input, N'ñ', N'n');
    SET @Input = REPLACE(@Input, N'ö', N'o');
    SET @Input = REPLACE(@Input, N'ó', N'o');
    SET @Input = REPLACE(@Input, N'ò', N'o');
    SET @Input = REPLACE(@Input, N'ô', N'o');
    SET @Input = REPLACE(@Input, N'ú', N'u');
    SET @Input = REPLACE(@Input, N'ù', N'u');
    SET @Input = REPLACE(@Input, N'ü', N'u');
    SET @Input = REPLACE(@Input, N'û', N'u');
    SET @Input = REPLACE(@Input, N'ç', N'c');
    SET @Input = REPLACE(@Input, N'ß', N's');
    SET @Input = REPLACE(@Input, N'ø', N'o');
    SET @Input = REPLACE(@Input, N'æ', N'ae');
    SET @Input = REPLACE(@Input, N'œ', N'oe');

    -- Uppercase variants
    SET @Input = REPLACE(@Input, N'Ï', N'I');
    SET @Input = REPLACE(@Input, N'Í', N'I');
    SET @Input = REPLACE(@Input, N'Ì', N'I');
    SET @Input = REPLACE(@Input, N'Î', N'I');
    SET @Input = REPLACE(@Input, N'É', N'E');
    SET @Input = REPLACE(@Input, N'È', N'E');
    SET @Input = REPLACE(@Input, N'Ê', N'E');
    SET @Input = REPLACE(@Input, N'Ë', N'E');
    SET @Input = REPLACE(@Input, N'Á', N'A');
    SET @Input = REPLACE(@Input, N'À', N'A');
    SET @Input = REPLACE(@Input, N'Â', N'A');
    SET @Input = REPLACE(@Input, N'Ä', N'A');
    SET @Input = REPLACE(@Input, N'Ã', N'A');
    SET @Input = REPLACE(@Input, N'Ñ', N'N');
    SET @Input = REPLACE(@Input, N'Ö', N'O');
    SET @Input = REPLACE(@Input, N'Ó', N'O');
    SET @Input = REPLACE(@Input, N'Ò', N'O');
    SET @Input = REPLACE(@Input, N'Ô', N'O');
    SET @Input = REPLACE(@Input, N'Ú', N'U');
    SET @Input = REPLACE(@Input, N'Ù', N'U');
    SET @Input = REPLACE(@Input, N'Ü', N'U');
    SET @Input = REPLACE(@Input, N'Û', N'U');
    SET @Input = REPLACE(@Input, N'Ç', N'C');
    SET @Input = REPLACE(@Input, N'Ø', N'O');
    SET @Input = REPLACE(@Input, N'Æ', N'Ae');
    SET @Input = REPLACE(@Input, N'Œ', N'Oe');

    -- Step 2: keep only A–Z, a–z, 0–9
    SET @Len = LEN(@Input);
    SET @i = 1;
    WHILE @i <= @Len
    BEGIN
        SET @c = SUBSTRING(@Input, @i, 1);
        SET @u = UNICODE(@c);
        IF (@u >= 65 AND @u <= 90)    -- A-Z
           OR (@u >= 97 AND @u <= 122) -- a-z
           OR (@u >= 48 AND @u <= 57)   -- 0-9
            SET @Result = @Result + @c;
        SET @i = @i + 1;
    END

    RETURN CAST(LTRIM(RTRIM(@Result)) AS VARCHAR(255));
END
GO
