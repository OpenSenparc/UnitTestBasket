using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadAndAsyncTests
{
    [TestClass]
    public class FuncAsyncTests
    {
        object objLock = new object();

        /// <summary>
        /// 测试 Func 异步方法的等待情况
        /// </summary>
        [TestMethod]
        public async Task FuncAsyncTest()
        {
            int index = 0;//线程完成计数
            var testCount = 5;//并发测试线程数
            var delayMs = 300;//线程延迟好描述
            var lastRunTime = SystemTime.Now;
            Func<DateTimeOffset,Task<int>> funcAsyncTest = async time =>
            {
                lastRunTime = time;//记录当前时间
                Console.WriteLine($"{SystemTime.Now.ToString("ss.ffff")}\t进入 funcAsyncTest ");
                await Task.Delay(delayMs);//延迟

                Console.WriteLine($"{SystemTime.Now.ToString("ss.ffff")}\t[{index}]延迟结束，开始执行 funcAsyncTest ");

                lock (objLock)
                {
                    index++;
                }
                return index;
            };

            Console.WriteLine("==== 不等待 ====");
            for (int i = 0; i < testCount; i++)
            {
                _ = funcAsyncTest(SystemTime.Now);//不等待，直接执行下一步
                Assert.IsTrue((SystemTime.Now - lastRunTime).TotalMilliseconds < delayMs);//循环间隔时间非常短
            }

            while (index < testCount)
            {
                //等待线程执行完毕（这里不使用Task.Wait()以及线程信号灯）
            }

            index = 0;
            Console.WriteLine("==== 等待 ====");

            for (int i = 0; i < testCount; i++)
            {
                await funcAsyncTest(SystemTime.Now);//会进行等待
                Assert.IsTrue((SystemTime.Now - lastRunTime).TotalMilliseconds >= delayMs);//循环间隔时间取决于线程内延迟情况
            }

            while (index < testCount)
            {
                //等待线程执行完毕（这里不使用Task.Wait()以及线程信号灯）
            }
        }
    }
}
