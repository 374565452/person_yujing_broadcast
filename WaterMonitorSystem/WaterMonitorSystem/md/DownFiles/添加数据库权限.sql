--添加只允许访问指定表的用户:
exec sp_addlogin 'zxct','zxct','WaterMonitorSystemDB'
 
--添加到数据库
exec sp_grantdbaccess 'zxct'
 
--分配整表权限
GRANT SELECT , INSERT , UPDATE , DELETE ON Crop TO zxct
 
--分配权限到具体的列
--GRANT SELECT , UPDATE ON table1(id,AA) TO zxct