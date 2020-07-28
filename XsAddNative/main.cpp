#include <iostream>
#include <array>
#include <vector>
#include <memory>
#include "xsadd.h"
#include <iomanip>
#include <string_view>

void gen(uint32_t seed,uint32_t mul_step,const char* base_step){
    using namespace std;

    xsadd_t rnd{};
    xsadd_init(&rnd,seed);

    xsadd_jump(&rnd,mul_step,base_step);

    cout<<"("<<seed<<','<<mul_step<<",\""<<base_step<<"\",new []{";

    for (int i = 0; i < 4; ++i) {
        cout<<rnd.state[i]<<"u,";
    }

    cout<<"}),"<<endl;

}

int main()
{
    using namespace std;

    char buffer[33];
    xsadd_calculate_jump_polynomial(buffer,1,"0x1000000000000000000000000");

    std::cout<<buffer<<std::endl;






}

