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
    xsadd_t state;
    xsadd_init(&state,42);

    for (int i = 0; i < 4; ++i) {
        std::cout<<state.state[i]<<std::endl;
    }

    std::cout<<std::endl;

    xsadd_jump(&state,4,"0x40000000");

    for (int i = 0; i < 4; ++i) {
        std::cout<<state.state[i]<<std::endl;
    }

    char buff[33];
    xsadd_calculate_jump_polynomial(buff,4,"0x40000000");
    std::cout<<std::endl<<buff<<std::endl;




}

