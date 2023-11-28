# Unity脚本

## C#接口
接口强调的是定义而非具体实现,对于通用的功能不应每个实体去实现接口,
会导致大量重复代码,例如**发射闪电**这么一个功能,封装具体游戏逻辑功能即可.

若想保留接口的定义,可以封装一个工具类来实现接口,从而保留抽象性.
```
   public class LightningLauncher : ILightningLauncher
   {
   }
```
## 协程(Coroutine)和UniTask
   依赖生命周期的脚本中尽量使用coroutine,因为携程逻辑开启和结束和mono脚本生命周期
 绑定,UniTask的异步是全局运行即使脚本被销毁依然执行
 


