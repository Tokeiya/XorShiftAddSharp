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

    gen(42,4,"0x40000000");
    gen(42,0x40000000,"4");

    gen(114514,0xFFFFFFFF,"0x100000000");
    gen(114514,0xFFFFFFFF,"100000000");








}

