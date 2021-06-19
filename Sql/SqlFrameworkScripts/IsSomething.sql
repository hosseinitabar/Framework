create function IsSomething (@text nvarchar(max))
returns bit
as
begin
	declare @result bit = 1;
	if @text is null
	begin
		set @result = 0;
	end
	else if ltrim(rtrim(@text)) = ''
	begin
		set @result = 0;
	end
	return @result;
end