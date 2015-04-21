#ifndef __TTTOOLBOX_THREADSAFE_QUEUE_HPP
#define __TTTOOLBOX_THREADSAFE_QUEUE_HPP

/********************************************
 * @copyright, Anthony Williams,
 * C++ Concurrency in Action, p74
 *********************************************/

#include <mutex>
#include <condition_variable>
#include <queue>
#include <memory>

namespace TTToolbox{

template<typename T>
class threadsafe_queue {
private:
    mutable std::mutex mutex_;
    std::condition_variable cond_;
    std::queue<T> q_;
public:
    threadsafe_queue()
    {}

    threadsafe_queue(threadsafe_queue const& rhs)
		:mutex_{}, cond_{}
    {
        std::lock_guard<std::mutex> lk(rhs.mutex_);
        q_ = rhs.q_;
    }

    void push(const T &new_value)
    {
        std::lock_guard<std::mutex> lk(mutex_);
        q_.push(new_value);
        cond_.notify_one();
    }

    void wait_and_pop(T& value)
    {
        std::unique_lock<std::mutex> lk(mutex_);
        cond_.wait(lk,[this]{return !q_.empty();});
        value=q_.front();
        q_.pop();
    }

    std::shared_ptr<T> wait_and_pop()
    {
        std::unique_lock<std::mutex> lk(mutex_);
        cond_.wait(lk,[this]{return !q_.empty();});
        std::shared_ptr<T> res(std::make_shared<T>(q_.front()));
        q_.pop();
        return res;
    }

    bool try_pop(T& value)
    {
        std::lock_guard<std::mutex> lk(mutex_);
        if(q_.empty())
            return false;
        value=q_.front();
        q_.pop();
		return true;
    }

    std::shared_ptr<T> try_pop()
    {
        std::lock_guard<std::mutex> lk(mutex_);
        if(q_.empty())
            return std::shared_ptr<T>();
        std::shared_ptr<T> res(std::make_shared<T>(q_.front()));
        q_.pop();
        return res;
    }

    bool empty() const
    {
        std::lock_guard<std::mutex> lk(mutex_);
        return q_.empty();
    }
};

}//namespace
#endif
