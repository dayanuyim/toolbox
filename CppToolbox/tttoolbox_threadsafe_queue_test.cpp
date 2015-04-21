#include <UnitTest++.h>
#include <iostream>
#include <string>
#include <thread>
#include <unistd.h>

#include "tttoolbox_threadsafe_queue.hpp"

using namespace std;
using namespace TTToolbox;

SUITE(SuiteName)
{
	TEST(TestName)
	{
		const int NUM = 10000;
		threadsafe_queue<int> q;
		//queue<int> q;
		int cc = 0;
		time_t begin = time(0);

		thread wth([&]{
				int val = 0;
				for(int i = 0; i < NUM; ++i){
					q.push(val++);
				}
			});

		thread rth([&]{
				int val = 0;
				while(true){
					if(q.try_pop(val))
						++cc;
					/*
					if(!q.empty()){
						q.pop();
						++cc;
					}
					*/

					if(time(0) - begin > 3)
						break;
				}
			});
					
		wth.join();
		rth.join();

		CHECK_EQUAL(NUM, cc);
	}
}

int main()
{
	try{
		UnitTest::RunAllTests();
	}
	catch(exception &ex){
		cerr << "system error: " << ex.what() << endl;
	}
	catch(...){
		cerr << "system error: " << "unknown" << endl;
	}

	return 0;
}
