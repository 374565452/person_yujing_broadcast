--���ֻ�������ָ������û�:
exec sp_addlogin 'zxct','zxct','WaterMonitorSystemDB'
 
--��ӵ����ݿ�
exec sp_grantdbaccess 'zxct'
 
--��������Ȩ��
GRANT SELECT , INSERT , UPDATE , DELETE ON Crop TO zxct
 
--����Ȩ�޵��������
--GRANT SELECT , UPDATE ON table1(id,AA) TO zxct