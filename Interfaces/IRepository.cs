namespace me.admin.api.Interfaces;

public interface IRepository<T> where T : class
{
	// 获取所有实体
	Task<List<T>> GetAll();

	// 根据ID获取实体
	Task<T?> GetById(string id);

	// 插入新的实体
	Task Insert(T entity);

	// 更新实体
	Task Update(T entity);

	// 删除实体
	Task Delete(string id);
}