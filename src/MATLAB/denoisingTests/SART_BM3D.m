

nProjections = length(Projections);

angleStep=360.0/nProjections;

sizeProjections = length(Projections{1});

jumpSize =floor( nProjections/4);

reconImageSIRT=reconImage;
clear filter;
B=0:(sizeProjections/2);
filter(1:length(B)) =B;
B=(sizeProjections/2):-1:1;
filter(length(filter):length(filter)+length(B)-1) =B;

filter=filter./(length(filter)/2)^2*3.14;

R_Thresh = (sizeProjections/2.1)^2;
centerI= sizeProjections/2;
centerJ= sizeProjections/2;
mask=zeros([sizeProjections sizeProjections]);

for I=1:sizeProjections
        for J=1:sizeProjections
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1
                mask(I,J)=1;
            end
        end
end
maskIndex = find(mask==1);

reconImageSIRT(maskIndex)=0;
 
regularization = Poisson_denoising_Anscombe_exact_unbiased_inverse(reconImageSIRT) - reconImageSIRT;

addition = zeros([sizeProjections sizeProjections]);
angleI=1;
for K=1:5
    
    reconImageSIRT(maskIndex)=0;
    
    for I=1:nProjections
        
        if (mod(I,5)==0)
            regularization =( Poisson_denoising_Anscombe_exact_unbiased_inverse(reconImageSIRT) - reconImageSIRT)/10;
        end
        angleI= mod(angleI+jumpSize + randi(15,1)-1,nProjections)+1;
        
        projection = Projections{angleI};

        tempImage = imrotate(reconImageSIRT,-1*angleStep*angleI,'bilinear','crop'); 
        
        forward = sum(tempImage);
        
        diff=(projection -forward)/sizeProjections;
   
        for J=1:sizeProjections
           addition(:,J)= diff(J);
        end

        addition=imrotate(addition,angleStep*angleI,'bilinear','crop');

        reconImageSIRT=reconImageSIRT + addition +.05* regularization;
    end
  
end

figure;imagesc(reconImageSIRT);colormap gray;


double(max(D(:)))/max(reconImageSIRT(:))