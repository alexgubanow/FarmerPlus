@echo Wait for clear database...
@E:\projects\farmer\farmer\bin\x86\Debug\psql_utils\psql.exe -h 37.57.131.238 -p 5432 -U postgres -a -f E:\backupdb\trun.sql
@echo Done!
@pause
